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
  [ContentType("text")]
  public class AutoExpandRegionsListener : IWpfTextViewCreationListener {
    [Import]
    private IVsOutliningManagerService outlining = null;
    public void TextViewCreated(IWpfTextView textView) {
      var expandMode = VsfSettings.AutoExpandRegions;
      var manager = outlining.GetOutliningManager(textView);
      if ( manager != null ) {
        textView.Properties.GetOrCreateSingletonProperty(
          () => new AutoExpander(textView, manager, expandMode)
          );
      }
    }
  }

  public class AutoExpander {
    private IWpfTextView theView;
    private IVsOutliningManager outliningManager;
    private AutoExpandMode expandMode;

    public AutoExpander(
          IWpfTextView textView, 
          IVsOutliningManager outlining,
          AutoExpandMode mode) {
      this.expandMode = mode;
      this.theView = textView;
      this.outliningManager = outlining;

      this.theView.Closed += OnViewClosed;
      VsfSettings.SettingsUpdated += OnSettingsUpdated;
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

    private void OnSettingsUpdated(object sender, EventArgs e) {
      if ( VsfSettings.AutoExpandRegions == AutoExpandMode.Disable ) {
        this.outliningManager.Enabled = false;
      } else {
        this.outliningManager.Enabled = true;
      }
    }

    private void OnViewClosed(object sender, EventArgs e) {
      VsfSettings.SettingsUpdated -= OnSettingsUpdated;
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
