using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Text {

  class RainbowClassifier : IClassifier, IDisposable {
    private ITextBuffer theBuffer;
    private IClassificationType[] rainbowTags;
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    private const int MAX_DEPTH = 4;
    private LanguageInfo language;
    private object updateLock = new object();
    private Dispatcher dispatcher;
    private BraceCache braceCache;

#pragma warning disable 67
    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

    internal RainbowClassifier(
          ITextBuffer buffer,
          IClassificationTypeRegistryService registry) {
      this.theBuffer = buffer;
      rainbowTags = new IClassificationType[MAX_DEPTH];

      for ( int i = 0; i < MAX_DEPTH; i++ ) {
        rainbowTags[i] = registry.GetClassificationType(Constants.RAINBOW + (i + 1));
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

    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
      List<ClassificationSpan> result = new List<ClassificationSpan>();
      if ( !VsfSettings.RainbowTagsEnabled ) {
        return result;
      }
      if ( span.Length == 0 ) {
        return result;
      }
      ITextSnapshot snapshot = span.Snapshot;
      if ( braceCache == null || braceCache.Snapshot != snapshot ) {
        return result;
      }
      foreach ( var brace in braceCache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)) ) {
        var tag = rainbowTags[brace.Depth % MAX_DEPTH];
        var nspan = new SnapshotSpan(snapshot, brace.Position, 1);
        result.Add(new ClassificationSpan(nspan, tag));
      }
      return result;
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
        // only invalidate the spans
        // containing all the positions of braces from the start point, leave
        // the rest alone
        if ( notify ) {
          foreach ( var brace in newCache.BracesFromPosition(startPoint.Position) ) {
            NotifyUpdateTags(new SnapshotSpan(startPoint.Snapshot, brace.Position, 1));
          }
        }
      }
    }

    private void ExtractBracesFromLastBrace(
          ITextSnapshot snapshot, int changePos, 
          BraceCache currentBraces, BraceCache newBraces) {
      int startPosition = 0;

      // figure out where to start parsing again
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
      var tempEvent = this.ClassificationChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new ClassificationChangedEventArgs(span));
      }
    }
  }
}