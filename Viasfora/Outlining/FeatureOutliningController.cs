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

    public FeatureOutliningController(ITextView view, IVsOutliningManager manager) {
      this.theView = view;
      this.outliningManager = manager;

      this.theView.Closed += OnViewClosed;
    }

    private void OnViewClosed(Object sender, EventArgs e) {
      if ( this.theView != null ) {
        this.theView.Closed -= OnViewClosed;
        this.theView = null;
      }
      this.outliningManager = null;
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
