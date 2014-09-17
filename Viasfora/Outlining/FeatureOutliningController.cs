using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using IVsOutliningManager = Microsoft.VisualStudio.Text.Outlining.IOutliningManager;
using IVsOutliningManagerService = Microsoft.VisualStudio.Text.Outlining.IOutliningManagerService;
using Microsoft.VisualStudio.Text;
using System.Windows.Threading;

namespace Winterdom.Viasfora.Outlining {
  [Export(typeof(IWpfTextViewCreationListener))]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [Name("Viasfora.outlining.feature.controller")]
  [ContentType("any")]
  public class FeatureOutliningControllerListener : IWpfTextViewCreationListener {
    [Import]
    private IVsOutliningManagerService outlining = null;

    public void TextViewCreated(IWpfTextView textView) {
      var manager = outlining.GetOutliningManager(textView);
      if ( manager != null ) {
        textView.Properties.GetOrCreateSingletonProperty(
          () => new FeatureOutliningController(textView, manager)
            as IFeatureOutliningController
        );
      }
    }
  }


  public class FeatureOutliningController : IFeatureOutliningController {
    private ITextView theView;
    private IVsOutliningManager outliningManager;
    private Dispatcher currentDispatcher;
    private DispatcherTimer timer;

    public FeatureOutliningController(ITextView view, IVsOutliningManager manager) {
      this.theView = view;
      this.outliningManager = manager;
      this.currentDispatcher = Dispatcher.CurrentDispatcher;
      this.timer = new DispatcherTimer(DispatcherPriority.Background, this.currentDispatcher);
      this.timer.Tick += OnTimerTick;

      this.theView.Closed += OnViewClosed;
    }

    private void OnViewClosed(Object sender, EventArgs e) {
      if ( this.theView != null ) {
        this.theView.Closed -= OnViewClosed;
        this.theView.LayoutChanged -= OnTextViewLayoutChanged;
        this.theView = null;
      }
      if ( this.timer != null ) {
        this.timer.Stop();
        this.timer = null;
      }
      this.outliningManager = null;
      this.currentDispatcher = null;
    }

    public static IFeatureOutliningController Get(ITextView view) {
      return view.Get<IFeatureOutliningController>();
    }

    public void CollapseRegions() {
      var buffer = this.theView.TextBuffer;
      var outlining = FeatureOutliningManager.Get(this.theView.TextBuffer);
      var allDoc = buffer.CurrentSnapshot.GetSpan();

      var regions = outlining.GetTags(new NormalizedSnapshotSpanCollection(allDoc));
      foreach ( var regionSpan in regions ) {
        CollapseRegion(regionSpan);
      }
    }
    public void RemoveRegions() {
      var buffer = this.theView.TextBuffer;
      var outlining = FeatureOutliningManager.Get(this.theView.TextBuffer);

      this.theView.LayoutChanged += OnTextViewLayoutChanged;
      outlining.RemoveAll(buffer.CurrentSnapshot);
    }

    private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      this.theView.LayoutChanged -= OnTextViewLayoutChanged;
      this.timer.Interval = TimeSpan.FromMilliseconds(100);
      this.timer.Start();
    }

    private void OnTimerTick(object sender, EventArgs e) {
      if ( this.theView == null || this.theView.InLayout ) {
        return;
      }
      this.timer.Stop();
      var caretPos = this.theView.Caret.Position.BufferPosition;
      var caretLine = this.theView.GetTextViewLineContainingBufferPosition(caretPos);
      this.theView.DisplayTextLineContainingBufferPosition(
        caretPos,
        (this.theView.ViewportHeight + caretLine.TextHeight) / 2,
        ViewRelativePosition.Top
      );
    }

    private void CollapseRegion(SnapshotSpan regionSpan) {
      var collapsible = this.outliningManager.GetAllRegions(regionSpan);
      foreach ( var c in collapsible ) {
        if ( c.Extent.GetSpan(regionSpan.Snapshot) == regionSpan ) {
          var result = this.outliningManager.TryCollapse(c);
          if ( result == null || !result.IsCollapsed ) {
            // try again
            this.outliningManager.TryCollapse(c);
          }
        }
      }
    }
  }
}
