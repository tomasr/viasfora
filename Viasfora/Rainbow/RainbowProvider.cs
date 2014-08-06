using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Rainbow {

  class RainbowProvider  {
    public ITextBuffer TextBuffer { get; private set; }
    public BraceCache BraceCache { get; private set; }
    public ITextView TextView { get; private set; }
    public IClassificationTypeRegistryService Registry { get; private set; }
    public ILanguageFactory LanguageFactory { get; private set; }
    public IVsfSettings Settings { get; private set; }
    public RainbowTaggerProvider Provider { get; private set; }
    public Dispatcher Dispatcher { get; private set; }
    public RainbowColorTagger ColorTagger { get; private set; }

    private object updateLock = new object();
    private DispatcherTimer dispatcherTimer;
    private int updatePendingFrom;

    internal RainbowProvider(
          ITextView view,
          ITextBuffer buffer,
          RainbowTaggerProvider provider) {
      this.TextView = view;
      this.TextBuffer = buffer;
      this.Registry = provider.ClassificationRegistry;
      this.LanguageFactory = provider.LanguageFactory;
      this.Settings = provider.Settings;
      this.ColorTagger = new RainbowColorTagger(this);

      SetLanguage(buffer.ContentType);

      this.updatePendingFrom = -1;
      this.TextView.Closed += OnViewClosed;
      this.TextBuffer.ChangedLowPriority += this.BufferChanged;
      this.TextBuffer.ContentTypeChanged += this.ContentTypeChanged;
      this.Settings.SettingsChanged += this.OnSettingsChanged;
      this.Dispatcher = Dispatcher.CurrentDispatcher;

      UpdateBraceList(new SnapshotPoint(buffer.CurrentSnapshot, 0));
    }

    public static bool TryMapCaretToBuffer(ITextView view, out SnapshotPoint pos) {
      var caret = view.Caret.Position.BufferPosition;
      return TryMapPosToBuffer(view, caret, out pos);
    }
    public static bool TryMapPosToBuffer(ITextView view, SnapshotPoint viewPos, out SnapshotPoint pos) {
      pos = new SnapshotPoint();
      var result = view.BufferGraph.MapDownToFirstMatch(
        viewPos, PointTrackingMode.Negative,
        snapshot => snapshot.TextBuffer.Has<RainbowProvider>(),
        PositionAffinity.Successor
        );
      if ( result != null ) {
        pos = result.Value;
        return true;
      }
      return false;
    }
    public static bool TryMapToView(ITextView view, SnapshotPoint pos, out SnapshotPoint result) {
      result = new SnapshotPoint();
      var target = view.TextBuffer;
      var temp = view.BufferGraph.MapUpToBuffer(
        pos, PointTrackingMode.Negative,
        PositionAffinity.Successor, target
      );
      if ( temp != null ) {
        result = temp.Value;
        return true;
      }
      return false;
    }

    private void OnViewClosed(object sender, EventArgs e) {
      if ( this.TextView != null ) {
        this.TextView.Closed -= OnViewClosed;
        this.TextView = null;
      }
      if ( TextBuffer != null ) {
        Settings.SettingsChanged -= OnSettingsChanged;
        TextBuffer.ChangedLowPriority -= this.BufferChanged;
        TextBuffer.ContentTypeChanged -= this.ContentTypeChanged;
        TextBuffer = null;
      }
      this.Dispatcher = null;
      if ( this.dispatcherTimer != null ) {
        this.dispatcherTimer.Stop();
        this.dispatcherTimer = null;
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
      this.BraceCache.Invalidate(startPoint);
      SynchronousUpdate(notifyUpdate, startPoint);
    }

    private void SynchronousUpdate(bool notify, SnapshotPoint startPoint) {
      lock ( updateLock ) {
        if ( notify ) {
          // only change the update mark if the current change is *before*
          // a pending one, or there are no notifications pending
          if ( this.updatePendingFrom < 0 || this.updatePendingFrom > startPoint.Position ) {
            this.updatePendingFrom = startPoint.Position;
          }
          ScheduleUpdate();
        }
      }
    }

    private void ScheduleUpdate() {
      if ( TextBuffer == null ) {
        return;
      }
      if ( dispatcherTimer == null ) {
        dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher);
        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
        dispatcherTimer.Tick += OnScheduledUpdate;
      }
      dispatcherTimer.Stop();
      dispatcherTimer.Start();
    }

    private void OnScheduledUpdate(object sender, EventArgs e) {
      if ( TextBuffer == null ) return;
      try {
        dispatcherTimer.Stop();
        FireTagsChanged();
      } catch {
      }
    }

    private void FireTagsChanged() {
      var snapshot = BraceCache.Snapshot;
      int upd;
      lock ( this.updateLock ) {
        upd = this.updatePendingFrom;
        this.updatePendingFrom = -1;
      }
      if ( upd < 0 ) return;
      var startPoint = new SnapshotPoint(snapshot, upd);
      NotifyUpdateTags(new SnapshotSpan(startPoint, snapshot.Length - startPoint.Position));
      //foreach ( var brace in braceCache.BracesFromPosition(upd) ) {
      //  NotifyUpdateTags(new SnapshotSpan(snapshot, brace.Position, 1));
      //}
    }

    private void SetLanguage(IContentType contentType) {
      if ( TextBuffer != null ) {
        var lang = LanguageFactory.TryCreateLanguage(contentType);
        this.BraceCache = new BraceCache(this.TextBuffer.CurrentSnapshot, lang);
      }
    }

    void OnSettingsChanged(object sender, EventArgs e) {
      this.UpdateBraceList(new SnapshotPoint(this.TextBuffer.CurrentSnapshot, 0));
    }

    private void BufferChanged(object sender, TextContentChangedEventArgs e) {
      if ( Settings.RainbowTagsEnabled ) {
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
      this.ColorTagger.NotifyUpdateTags(span);
    }
  }
}