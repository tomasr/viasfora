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

  class RainbowTagger : ITagger<ClassificationTag>, IDisposable {
    private ITextBuffer theBuffer;
    private ITextView theView;
    private ClassificationTag[] rainbowTags;
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    private const String BRACE_CHARS = "(){}[]";
    private const int MAX_DEPTH = 4;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal RainbowTagger(
          ITextBuffer buffer, ITextView textView,
          IClassificationTypeRegistryService registry) {
      this.theView = textView;
      this.theBuffer = buffer;
      rainbowTags = new ClassificationTag[MAX_DEPTH];
      for ( int i = 0; i < MAX_DEPTH; i++ ) {
        rainbowTags[i] = new ClassificationTag(
          registry.GetClassificationType(Constants.RAINBOW + (i + 1)));
      }
      for ( int i = 0; i < BRACE_CHARS.Length; i += 2 ) {
        braceList.Add(BRACE_CHARS[i], BRACE_CHARS[i + 1]);
      }

      this.theBuffer.Changed += BufferChanged;
      this.theView.LayoutChanged += ViewLayoutChanged;
      VsfSettings.SettingsUpdated += this.OnSettingsUpdated;
    }

    public void Dispose() {
      if ( theBuffer != null ) {
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        theBuffer = null;
      }
    }

    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( !VsfSettings.RainbowTagsEnabled ) yield break;
      if ( spans.Count == 0 ) {
        yield break;
      }
      ITextSnapshot snapshot = spans[0].Snapshot;
      if ( !IsSupported(snapshot.ContentType) ) {
        yield break;
      }
      SnapshotPoint startPoint = new SnapshotPoint(snapshot, 0);
      //SnapshotPoint startPoint = new SnapshotPoint(snapshot, spans[0].Start);
      foreach ( var tagSpan in LookForMatchingPairs(startPoint) ) {
        yield return tagSpan;
      }
    }

    class Pair {
      public char Brace { get; set; }
      public int Depth { get; set; }
      public int Open { get; set; }
      public int Close { get; set; }
    }

    private IEnumerable<ITagSpan<ClassificationTag>> LookForMatchingPairs(SnapshotPoint startPoint) {
      Stack<Pair> pairs = new Stack<Pair>();
      ITextSnapshot snapshot = startPoint.Snapshot;

      FindBracePairs(startPoint, pairs);

      foreach ( var p in pairs ) {
        var tag = this.rainbowTags[p.Depth % MAX_DEPTH];
        var span = new SnapshotSpan(snapshot, p.Open, 1);
        yield return new TagSpan<ClassificationTag>(span, tag);
        if ( p.Close >= 0 ) {
          span = new SnapshotSpan(snapshot, p.Close, 1);
          yield return new TagSpan<ClassificationTag>(span, tag);
        }
      }
    }

    private void FindBracePairs(SnapshotPoint startPoint, Stack<Pair> pairs)  {
      ITextSnapshot snapshot = startPoint.Snapshot;

      int depth = 0;
      BraceExtractor extractor =  new BraceExtractor(startPoint, BRACE_CHARS);
      while ( true ) {
        SnapshotPoint? pt = extractor.NextBrace();
        if ( pt.HasValue ) {
          char ch = pt.Value.GetChar();
          if ( IsOpeningBrace(ch) ) {
            pairs.Push(new Pair {
              Brace = ch, Depth = depth,
              Open = pt.Value.Position, Close = -1
            });
            depth++;
          } else if ( IsClosingBrace(ch) ) {
            if ( MatchBrace(pairs, ch, pt.Value.Position) )
              depth--;
          }
        } else {
          break;
        }
      }
    }

    private bool MatchBrace(Stack<Pair> pairs, char ch, int pos) {
      foreach ( var p in pairs ) {
        if ( p.Close < 0 && braceList[p.Brace] == ch ) {
          p.Close = pos;
          return true;
        }
      }
      return false;
    }

    private bool IsClosingBrace(char ch) {
      return braceList.Values.Contains(ch);
    }

    private bool IsOpeningBrace(char ch) {
      return braceList.ContainsKey(ch);
    }

    void OnSettingsUpdated(object sender, EventArgs e) {
      UpdateTags(theBuffer.CurrentSnapshot, 0);
    }

    private void BufferChanged(object sender, TextContentChangedEventArgs e) {
      UpdateTags(e.After, e.Changes[0].NewSpan.Start);
    }
    private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      if ( e.NewSnapshot != e.OldSnapshot ) {
        UpdateTags(e.NewSnapshot, 0);
      }
    }

    private void UpdateTags(ITextSnapshot snapshot, int startPosition) {
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, startPosition,
            snapshot.Length - startPosition)));
      }
    }

    private bool IsSupported(IContentType contentType) {
      return contentType.IsOfType(CSharp.ContentType)
          || contentType.IsOfType(Cpp.ContentType)
          || contentType.IsOfType(JScript.ContentType)
          || contentType.IsOfType(JScript.ContentTypeVS2012);
    }
  }

  class BraceExtractor {
    private ITextSnapshot snapshot;
    private String braceChars;
    private ITextSnapshotLine currentLine;
    private int pos;
    private int status;
    private const int ST_NORMAL = 0x00;
    private const int ST_COMMENT = 0x01;
    private const int ST_STRING = 0x02;
    private const int ST_MULTILINE = 0x80;

    public BraceExtractor(SnapshotPoint startPoint, String braceChars) {
      this.snapshot = startPoint.Snapshot;
      this.currentLine = this.snapshot.GetLineFromPosition(startPoint.Position);
      this.braceChars = braceChars;
      this.pos = 0;
      this.status = ST_NORMAL;
    }

    public SnapshotPoint? NextBrace() {
      while ( currentLine != null ) {
        SnapshotPoint? pt = this.NextBraceInLine();
        if ( pt != null ) {
          return pt;
        }
        if ( currentLine.LineNumber < snapshot.LineCount - 1 ) {
          currentLine = this.snapshot.GetLineFromLineNumber(currentLine.LineNumber+1);
          pos = 0;
        } else {
          currentLine = null;
        }
      }
      return null;
    }

    private SnapshotPoint? NextBraceInLine() {
      String text = currentLine.GetText();
      for ( ; pos < currentLine.Length; pos++ ) {
        switch ( this.status ) {
          case ST_NORMAL:
            if ( IsBrace(text[pos]) ) {
              return new SnapshotPoint(this.snapshot, currentLine.Start + pos++);
            }
            if ( pos > 0 && text[pos - 1] == '/' && text[pos] == '/' ) {
              // single line comment, ignore the rest
              pos = currentLine.Length;
            } else if ( pos > 0 && text[pos - 1] == '/' && text[pos] == '*' ) {
              // multiline comment
              this.status = ST_COMMENT | ST_MULTILINE;
            } else if ( text[pos] == '"' ) {
              this.status = ST_STRING;
            } else if ( pos > 0 && text[pos - 1] == '@' && text[pos] == '"' ) {
              this.status = ST_STRING | ST_MULTILINE;
            } 
            break;
          case ST_COMMENT | ST_MULTILINE:
            if ( pos > 0 && text[pos - 1] == '*' && text[pos] == '/' ) {
              this.status = ST_NORMAL;
            }
            break;
          case ST_STRING:
          case ST_STRING | ST_MULTILINE:
            if ( text[pos] == '"' ) {
              if ( !(pos > 0 && text[pos-1] == '\\') ) {
                this.status = ST_NORMAL;
              }
            }
            break;
        }
      }
      return null;
    }

    private bool IsBrace(char ch) {
      return this.braceChars.IndexOf(ch) >= 0;
    }

  }
}