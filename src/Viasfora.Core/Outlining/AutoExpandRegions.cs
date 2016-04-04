using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using IVsOutliningManager = Microsoft.VisualStudio.Text.Outlining.IOutliningManager;
using IVsOutliningManagerService = Microsoft.VisualStudio.Text.Outlining.IOutliningManagerService;

namespace Winterdom.Viasfora.Outlining {
  [Export(typeof(IWpfTextViewCreationListener))]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [Name("Viasfora.auto-expand-regions")]
  [ContentType(ContentTypes.Text)]
  public class AutoExpandRegionsListener : IWpfTextViewCreationListener {
    [Import]
    private IVsOutliningManagerService outlining = null;
    [Import]
    private IVsfSettings settings = null;
    public void TextViewCreated(IWpfTextView textView) {
      var manager = outlining.GetOutliningManager(textView);
      if ( manager != null ) {
        textView.Properties.GetOrCreateSingletonProperty(
          () => new AutoExpander(textView, manager, settings)
          );
      }
    }
  }

  public class AutoExpander {
    private IWpfTextView theView;
    private IVsOutliningManager outliningManager;
    private AutoExpandMode expandMode;
    private IVsfSettings settings;

    public AutoExpander(
          IWpfTextView textView, 
          IVsOutliningManager outlining,
          IVsfSettings settings) {
      this.settings = settings;
      this.theView = textView;
      this.outliningManager = outlining;
      this.expandMode = settings.AutoExpandRegions;

      this.theView.Closed += OnViewClosed;
      this.settings.SettingsChanged += OnSettingsChanged;

      if ( expandMode == AutoExpandMode.Disable ) {
        outlining.Enabled = false;
      } else if ( expandMode == AutoExpandMode.Expand ) {
        // in most cases, this is enough to 
        // expand all outlining as necessary.
        // However, it does not appear to work
        // if the solution is just opened
        // so take notice of when regions are
        // collapsed and do it again just in case
        // Try expanding it when the window gets focus
        // as a last chance for Visual Basic
        this.theView.LayoutChanged += OnLayoutChanged;
        this.outliningManager.RegionsCollapsed += OnRegionsCollapsed;
        this.theView.GotAggregateFocus += OnGotFocus;
      }
    }

    private void OnGotFocus(object sender, EventArgs e) {
      this.theView.GotAggregateFocus -= OnGotFocus;
      ExpandAll();
    }

    private void OnSettingsChanged(object sender, EventArgs e) {
      if ( settings.AutoExpandRegions == AutoExpandMode.Disable ) {
        this.outliningManager.Enabled = false;
      } else {
        this.outliningManager.Enabled = true;
      }
    }

    private void OnViewClosed(object sender, EventArgs e) {
      this.settings.SettingsChanged -= OnSettingsChanged;
      this.settings = null;
      this.outliningManager.RegionsCollapsed -= OnRegionsCollapsed;
      this.theView.LayoutChanged -= OnLayoutChanged;
      this.theView.GotAggregateFocus -= OnGotFocus;
      this.theView.Closed -= OnViewClosed;
      this.theView = null;
      this.outliningManager = null;
    }

    private void OnRegionsCollapsed(object sender, RegionsCollapsedEventArgs e) {
      this.outliningManager.RegionsCollapsed -= OnRegionsCollapsed;
      ExpandAll();
    }

    private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      this.theView.LayoutChanged -= OnLayoutChanged;
      ExpandAll();
    }

    private void ExpandAll() {
      var snapshot = theView.TextSnapshot;
      if ( snapshot != null ) {
        SnapshotSpan span = new SnapshotSpan(snapshot, 0, snapshot.Length);
        outliningManager.ExpandAll(span, collapsed => true);
      }
    }
  }
}
