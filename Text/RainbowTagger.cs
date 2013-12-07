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

  class RainbowTagger : ITagger<RainbowTag>, IDisposable {
    private ITextBuffer theBuffer;
    private RainbowTag[] rainbowTags;
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    private const int MAX_DEPTH = 4;
    private LanguageInfo language;
    private object updateLock = new object();
    private Dispatcher dispatcher;
    private BraceCache braceCache;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal RainbowTagger(
          ITextBuffer buffer,
          IClassificationTypeRegistryService registry) {
      this.theBuffer = buffer;
      rainbowTags = new RainbowTag[MAX_DEPTH];

      for ( int i = 0; i < MAX_DEPTH; i++ ) {
        rainbowTags[i] = new RainbowTag(
          registry.GetClassificationType(Constants.RAINBOW + (i + 1)));
      }

      SetLanguage(buffer.ContentType);

      this.theBuffer.ChangedLowPriority += this.BufferChanged;
      this.theBuffer.ContentTypeChanged += this.ContentTypeChanged;
      VsfSettings.SettingsUpdated += this.OnSettingsUpdated;
      this.dispatcher = Dispatcher.CurrentDispatcher;

      this.braceCache = new BraceCache(buffer.CurrentSnapshot);
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

    public IEnumerable<ITagSpan<RainbowTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( !VsfSettings.RainbowTagsEnabled ) {
        yield break;
      }
      if ( spans.Count == 0 ) {
        yield break;
      }
      ITextSnapshot snapshot = spans[0].Snapshot;
      if ( braceCache == null || braceCache.Snapshot != snapshot ) { 
        yield break;
      }
      foreach ( var brace in braceCache.BracesInSpans(spans) ) {
        var tag = rainbowTags[brace.Depth % MAX_DEPTH];
        var span = new SnapshotSpan(snapshot, brace.Position, 1);
        yield return new TagSpan<RainbowTag>(span, tag);
      }
    }

    private void UpdateBraceList(SnapshotPoint startPoint) {
      UpdateBraceList(startPoint, true);
    }
    private void UpdateBraceList(ITextSnapshot snapshot, INormalizedTextChangeCollection changes) {
      bool notify = true;
      var startPoint = new SnapshotPoint(snapshot, changes[0].NewSpan.Start);
      UpdateBraceList(startPoint, notify);
    }
    private void UpdateBraceList(SnapshotPoint startPoint, bool notifyUpdate) {
      ITextSnapshot snapshot = startPoint.Snapshot;
      BraceCache newCache = new BraceCache(snapshot);
      this.ExtractBracesFromLastBrace(snapshot, startPoint.Position, this.braceCache, newCache);
      SynchronousUpdate(notifyUpdate, startPoint, newCache);
    }

    private void SynchronousUpdate(bool notify, SnapshotPoint startPoint, BraceCache newCache) {
      lock ( updateLock ) {
        this.braceCache = newCache;
        // notifying other taggers that we changed something.
        // Unfortunately, this can be brutally slow, so
        // just invalidate the rest of the line, and the rest
        // will get updated "soon" (for some definition of soon)
        if ( notify ) {
          var line = startPoint.GetContainingLine();
          var span = new SnapshotSpan(startPoint, line.End - startPoint);
          dispatcher.BeginInvoke(
            new Action<SnapshotSpan>(s => NotifyUpdateTags(s)),
            DispatcherPriority.Background,
            span);
        }
      }
    }

    private void ExtractBracesFromLastBrace(
          ITextSnapshot snapshot, int changePos, 
          BraceCache currentBraces, BraceCache newBraces) {
      int startPosition = 0;

      // figure out where to start parsing again
      int lastLineNum = -1;
      Stack<BracePos> pairs = new Stack<BracePos>();
      foreach ( var r in currentBraces.AllBraces() ) {
        if ( r.Position >= changePos ) break;
        if ( IsOpeningBrace(r.Brace) ) {
          pairs.Push(r);
        } else {
          pairs.Pop();
        }
        startPosition = r.Position + 1;
        newBraces.Add(r);
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
          newBraces.Add(p);
        } else if ( IsClosingBrace(ch) && pairs.Count > 0 ) {
          BracePos p = pairs.Peek();
          if ( braceList[p.Brace] == ch ) {
            // yield closing brace
            pairs.Pop();
            BracePos c = new BracePos {
              Brace = ch, Depth = p.Depth,
              Position = pt.Position
            };
            newBraces.Add(c);
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
          UpdateBraceList(e.After, e.Changes);
        }
      }
    }

    private void ContentTypeChanged(object sender, ContentTypeChangedEventArgs e) {
      if ( e.BeforeContentType.TypeName != e.AfterContentType.TypeName ) {
        this.SetLanguage(e.AfterContentType);
        this.UpdateBraceList(new SnapshotPoint(e.After, 0));
      }
    }

    private void NotifyUpdateTags(SnapshotSpan span) {
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(span));
      }
    }
  }

  public class RainbowTag : IClassificationTag {
    public IClassificationType ClassificationType { get; private set; }
    public RainbowTag(IClassificationType classification) {
      this.ClassificationType = classification;
    }
  }

}