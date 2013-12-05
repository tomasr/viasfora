using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Threading;

namespace Winterdom.Viasfora.Text {

  class RainbowTagger : ITagger<ClassificationTag>, IDisposable {
    private ITextBuffer theBuffer;
    private ClassificationTag[] rainbowTags;
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    private const int MAX_DEPTH = 4;
    private LanguageInfo language;
    private ITextSnapshot currentVersion;
    private List<BracePos> bracesFound = new List<BracePos>();
    private object updateLock = new object();

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67


    internal RainbowTagger(
          ITextBuffer buffer,
          IClassificationTypeRegistryService registry) {
      this.theBuffer = buffer;
      rainbowTags = new ClassificationTag[MAX_DEPTH];

      for ( int i = 0; i < MAX_DEPTH; i++ ) {
        rainbowTags[i] = new ClassificationTag(
          registry.GetClassificationType(Constants.RAINBOW + (i + 1)));
      }

      SetLanguage(buffer.ContentType);

      this.theBuffer.ChangedLowPriority += this.BufferChanged;
      this.theBuffer.ContentTypeChanged += this.ContentTypeChanged;
      VsfSettings.SettingsUpdated += this.OnSettingsUpdated;

      UpdateBraceList(new SnapshotPoint(buffer.CurrentSnapshot, 0));
    }

    public void Dispose() {
      if ( theBuffer != null ) {
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        theBuffer.ChangedLowPriority -= this.BufferChanged;
        theBuffer.ContentTypeChanged -= this.ContentTypeChanged;
        theBuffer = null;
      }
    }

    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( !VsfSettings.RainbowTagsEnabled ) yield break;
      if ( spans.Count == 0 ) {
        yield break;
      }
      ITextSnapshot snapshot = spans[0].Snapshot;
      if ( this.bracesFound.Count <= 0 || currentVersion != snapshot ) { 
        yield break;
      }
      int startPosition = spans[0].Start;
      int endPosition = spans[spans.Count - 1].End;
      foreach ( var brace in bracesFound ) {
        // only return tags that are included in 
        // the requested spans
        if ( brace.Position > endPosition ) break;
        if ( brace.Position >= startPosition ) {
          var span = new SnapshotSpan(snapshot, brace.Position, 1);
          var tag = this.rainbowTags[brace.Depth % MAX_DEPTH];
          yield return new TagSpan<ClassificationTag>(span, tag);
        }
      }
    }

    private void UpdateBraceList(SnapshotPoint startPoint) {
      List<BracePos> newList = new List<BracePos>();
      ITextSnapshot snapshot = startPoint.Snapshot;
      this.ExtractBracesFromLastBrace(snapshot, startPoint.Position, this.bracesFound, newList);
      SynchronousUpdate(startPoint, newList);
    }

    private void SynchronousUpdate(SnapshotPoint startPoint, List<BracePos> newBraces) {
      lock ( updateLock ) {
        this.bracesFound = newBraces;
        currentVersion = startPoint.Snapshot;
        // notifying other taggers that we changed
        // turns out to be just too expensive most of the time
        // we're a fairly lazy extension, so it's ok if remainder
        // braces update after a second.
        NotifyUpdateTags(startPoint.Snapshot, startPoint.Position);
      }
    }

    class BracePos {
      public char Brace { get; set; }
      public int Depth { get; set; }
      public int Position { get; set; }
    }

    private void ExtractBracesFromLastBrace(
          ITextSnapshot snapshot, int changePos, 
          List<BracePos> currentBraces, List<BracePos> result) {
      int startPosition = 0;
      // figure out where to start parsing again
      Stack<BracePos> pairs = new Stack<BracePos>();
      foreach ( var r in currentBraces ) {
        if ( r.Position >= changePos ) break;
        if ( IsOpeningBrace(r.Brace) ) {
          pairs.Push(r);
        } else {
          pairs.Pop();
        }
        startPosition = r.Position+1;
        result.Add(r);
      }
      // now we have the state back to our original status
      // so that we can extract the remainder of the tags
      BraceExtractor extractor = new BraceExtractor(
        new SnapshotPoint(snapshot, startPosition), language);
      foreach ( var pt in extractor.All() ) {
        char ch = pt.GetChar();
        if ( IsOpeningBrace(ch) ) {
          BracePos p = new BracePos {
            Brace = ch, Depth = pairs.Count,
            Position = pt.Position
          };
          pairs.Push(p);
          result.Add(p);
        } else if ( IsClosingBrace(ch) && pairs.Count > 0 ) {
          BracePos p = pairs.Peek();
          if ( braceList[p.Brace] == ch ) {
            // yield closing brace
            pairs.Pop();
            BracePos c = new BracePos {
              Brace = ch, Depth = p.Depth,
              Position = pt.Position
            };
            result.Add(c);
          }
        }
      }
    }

    private bool IsClosingBrace(char ch) {
      return braceList.Values.Contains(ch);
    }

    private bool IsOpeningBrace(char ch) {
      return braceList.ContainsKey(ch);
    }

    private void SetLanguage(IContentType contentType) {
      language = VsfPackage.LookupLanguage(contentType);
      this.braceList.Clear();
      String braceChars = language.BraceList;
      for ( int i = 0; i < braceChars.Length; i += 2 ) {
        this.braceList.Add(braceChars[i], braceChars[i + 1]);
      }
    }

    void OnSettingsUpdated(object sender, EventArgs e) {
      this.UpdateBraceList(new SnapshotPoint(this.theBuffer.CurrentSnapshot, 0));
    }

    private void BufferChanged(object sender, TextContentChangedEventArgs e) {
      if ( VsfSettings.RainbowTagsEnabled ) {
        // the snapshot changed, so we need to pretty much update
        // everything so that it matches.
        if ( e.Changes.Count > 0 ) {
          UpdateBraceList(new SnapshotPoint(e.After, e.Changes[0].NewSpan.Start));
        }
      }
    }

    private void ContentTypeChanged(object sender, ContentTypeChangedEventArgs e) {
      if ( e.BeforeContentType.TypeName != e.AfterContentType.TypeName ) {
        this.SetLanguage(e.AfterContentType);
        this.UpdateBraceList(new SnapshotPoint(e.After, 0));
      }
    }

    private void NotifyUpdateTags(ITextSnapshot snapshot, int startPosition) {
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, startPosition,
            snapshot.Length - startPosition)));
      }
    }
  }
}