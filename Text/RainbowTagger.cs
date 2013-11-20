using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  class RainbowTagger : ITagger<TextMarkerTag>, IDisposable {
    private ITextBuffer theBuffer;
    private ITextView theView;
    private TextMarkerTag[] rainbowTags;
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    private const int MAX_DEPTH = 4;
    private String braceChars = "(){}[]";

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal RainbowTagger(ITextBuffer buffer, ITextView textView) {
      this.theView = textView;
      theBuffer = buffer;
      rainbowTags = new TextMarkerTag[MAX_DEPTH];
      for ( int i = 0; i < MAX_DEPTH; i++ ) {
        rainbowTags[i] = new TextMarkerTag(Constants.RAINBOW + (i + 1));
      }
      for ( int i = 0; i < braceChars.Length; i += 2 ) {
        braceList.Add(braceChars[i], braceChars[i + 1]);
      }

      this.theBuffer.Changed += BufferChanged;
      this.theView.LayoutChanged += ViewLayoutChanged;
      VsfSettings.SettingsUpdated += this.OnSettingsUpdated;
    }

    public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      //if ( !VsfSettings.XmlMatchTagsEnabled ) yield break;
      if ( spans.Count == 0 ) {
        yield break;
      }
      ITextSnapshot snapshot = spans[0].Snapshot;
      if ( !IsSupported(snapshot.ContentType) ) {
        yield break;
      }
      if ( true /*VsfSettings.EscapeSeqHighlightEnabled*/ ) {
        foreach ( var tagSpan in LookForMatchingPairs(new SnapshotPoint(snapshot, 0)) ) {
          yield return tagSpan;
        }
      }
    }

    class Pair {
      public char Brace { get; set; }
      public int Open { get; set; }
      public int Close { get; set; }
    }
    private IEnumerable<ITagSpan<TextMarkerTag>> LookForMatchingPairs(SnapshotPoint startPoint) {
      Stack<Pair> pairs = new Stack<Pair>();
      ITextSnapshot snapshot = startPoint.Snapshot;
      int startLine = snapshot.GetLineNumberFromPosition(startPoint.Position);

      for ( int lineNr = startLine; lineNr < snapshot.LineCount; lineNr++ ) {
        ITextSnapshotLine line = snapshot.GetLineFromLineNumber(lineNr);
        String text = line.GetText();
        for ( int i = 0; i < line.Length; i++ ) {
          char ch = text[i];
          if ( IsOpeningBrace(ch) ) {
            pairs.Push(new Pair { Brace = ch, Open = line.Start + i, Close = -1 });
          } else if ( IsClosingBrace(ch) ) {
            MatchBrace(pairs, ch, line.Start + i);
          }
        }
      }

      int index = 0;
      foreach ( var p in pairs ) {
        var tag = this.rainbowTags[index % MAX_DEPTH];
        var span = new SnapshotSpan(snapshot, p.Open, 1);
        yield return new TagSpan<TextMarkerTag>(span, tag);
        if ( p.Close >= 0 ) {
          span = new SnapshotSpan(snapshot, p.Close, 1);
          yield return new TagSpan<TextMarkerTag>(span, tag);
        }
        index++;
      }
    }

    private void MatchBrace(Stack<Pair> pairs, char ch, int pos) {
      foreach ( var p in pairs ) {
        if ( p.Close < 0 && braceList[p.Brace] == ch ) {
          p.Close = pos;
          break;
        }
      }
    }

    private bool IsClosingBrace(char ch) {
      return braceList.Values.Contains(ch);
    }

    private bool IsOpeningBrace(char ch) {
      return braceList.ContainsKey(ch);
    }

    public void Dispose() {
      if ( theBuffer != null ) {
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        theBuffer = null;
      }
    }
    void OnSettingsUpdated(object sender, EventArgs e) {
      UpdateTags();
    }

    private void BufferChanged(object sender, TextContentChangedEventArgs e) {
      UpdateTags();
    }
    private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      if ( e.NewSnapshot != e.OldSnapshot ) {
        UpdateTags();
      }
    }

    private void UpdateTags() {
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(theBuffer.CurrentSnapshot, 0,
            theBuffer.CurrentSnapshot.Length)));
      }
    }

    private bool IsSupported(IContentType contentType) {
      return contentType.IsOfType(CSharp.ContentType)
          || contentType.IsOfType(Cpp.ContentType)
          || contentType.IsOfType(JScript.ContentType)
          || contentType.IsOfType(JScript.ContentTypeVS2012);
    }
  }

}
