using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Rainbow {

  class RainbowProvider : IWeakEventListener {
    public ITextBuffer TextBuffer { get; private set; }
    public ITextBufferBraces BufferBraces { get; private set; }
    public IClassificationTypeRegistryService Registry { get; private set; }
    public ILanguageFactory LanguageFactory { get; private set; }
    public IRainbowSettings Settings { get; private set; }
    public RainbowTaggerProvider Provider { get; private set; }
    public Dispatcher Dispatcher { get; private set; }
    public RainbowColorTagger ColorTagger { get; private set; }

    private object updateLock = new object();
    private DispatcherTimer dispatcherTimer;
    private int updatePendingFrom;

    internal RainbowProvider(
          ITextBuffer buffer,
          RainbowTaggerProvider provider) {
      this.TextBuffer = buffer;
      this.Registry = provider.ClassificationRegistry;
      this.LanguageFactory = provider.LanguageFactory;
      this.Settings = provider.Settings;
      this.ColorTagger = new RainbowColorTagger(this);

      SetLanguage(buffer.CurrentSnapshot);

      this.updatePendingFrom = -1;
      this.TextBuffer.ChangedLowPriority += this.BufferChanged;
      this.TextBuffer.ContentTypeChanged += this.ContentTypeChanged;
      this.Dispatcher = Dispatcher.CurrentDispatcher;
      VsfSettingsEventManager.AddListener(this.Settings, this);

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

    private void SettingsChanged() {
      // force recreating the TextBufferBraces instance
      SetLanguage(this.TextBuffer.CurrentSnapshot);
      this.UpdateBraceList(new SnapshotPoint(this.TextBuffer.CurrentSnapshot, 0));
    }

    private void UnsubscribeFromEvents() {
      if ( TextBuffer != null ) {
        // ensure that we remove the property so that
        // next time we create a new provider
        TextBuffer.Properties.RemoveProperty(GetType());
        TextBuffer.ChangedLowPriority -= this.BufferChanged;
        TextBuffer.ContentTypeChanged -= this.ContentTypeChanged;
        TextBuffer = null;
      }
      this.Dispatcher = null;
      if ( this.dispatcherTimer != null ) {
        this.dispatcherTimer.Stop();
        this.dispatcherTimer = null;
      }
      if ( Settings != null ) {
        VsfSettingsEventManager.RemoveListener(this.Settings, this);
        Settings = null;
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
      this.BufferBraces.Invalidate(startPoint);
      var restartPoint = startPoint;
      if ( this.BufferBraces.LastParsedPosition > 0 )
        restartPoint = new SnapshotPoint(startPoint.Snapshot, this.BufferBraces.LastParsedPosition);
      SynchronousUpdate(notifyUpdate, restartPoint);
    }

    private void SynchronousUpdate(bool notify, SnapshotPoint startPoint) {
      lock ( this.updateLock ) {
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
      if ( this.dispatcherTimer == null ) {
        this.dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher);
        this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
        this.dispatcherTimer.Tick += OnScheduledUpdate;
      }
      this.dispatcherTimer.Stop();
      this.dispatcherTimer.Start();
    }

    private void OnScheduledUpdate(object sender, EventArgs e) {
      if ( TextBuffer == null ) return;
      try {
        this.dispatcherTimer.Stop();
        FireTagsChanged();
      } catch {
      }
    }

    private void FireTagsChanged() {
      var snapshot = BufferBraces.Snapshot;
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

    private void SetLanguage(ITextSnapshot snapshot) {
      if ( TextBuffer != null ) {
        var lang = LanguageFactory.TryCreateLanguage(snapshot);
        var mode = Settings.RainbowColoringMode;
        this.BufferBraces = new TextBufferBraces(this.TextBuffer.CurrentSnapshot, lang, mode);
      }
    }

    private void BufferChanged(object sender, TextContentChangedEventArgs e) {
      if ( Settings.RainbowTagsEnabled ) {
        // the snapshot changed, so we need to pretty much update
        // everything so that it matches.
        if ( e.Changes.Count > 0 ) {
          UpdateBraceList(e.After, e.Changes);
        } else {
          // this can happen on the Razor editor,
          // where some changes on the buffer trigger
          // the buffer changed event, but the collection
          // is empty, so we just want to update the snapshot
          // so that when we're asked for the tags we respond
          // correctly to the message
          this.BufferBraces.UpdateSnapshot(e.After);
        }
      }
    }

    private void ContentTypeChanged(object sender, ContentTypeChangedEventArgs e) {
      if ( e.BeforeContentType.TypeName != e.AfterContentType.TypeName ) {
        this.SetLanguage(e.After);
        this.UpdateBraceList(new SnapshotPoint(e.After, 0));
      }
    }

    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
      // we only care about a single event
      this.SettingsChanged();
      return true;
    }

    private void NotifyUpdateTags(SnapshotSpan span) {
      this.ColorTagger.NotifyUpdateTags(span);
    }

  }
}