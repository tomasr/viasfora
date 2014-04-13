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

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IWpfTextViewCreationListener))]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [Name("Viasfora.auto-expand-regions")]
  [ContentType("text")]
  public class AutoExpandRegionsListener : IWpfTextViewCreationListener {
    [Import]
    private IVsOutliningManagerService outlining = null;
    public void TextViewCreated(IWpfTextView textView) {
      var manager = outlining.GetOutliningManager(textView);
      textView.Properties.GetOrCreateSingletonProperty(
        () => new AutoExpander(textView, manager)
        );
    }
  }

  public class AutoExpander {
    private IWpfTextView theView;
    private IVsOutliningManager outliningManager;

    public AutoExpander(IWpfTextView textView, IVsOutliningManager outlining) {
      this.theView = textView;
      this.theView.LayoutChanged += OnLayoutChanged;
      this.outliningManager = outlining;
    }

    private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      ExpandAll();
      this.theView.LayoutChanged -= OnLayoutChanged;
      this.theView = null;
      this.outliningManager = null;
    }

    private void ExpandAll() {
      var snapshot = theView.TextSnapshot;
      SnapshotSpan span = new SnapshotSpan(snapshot, 0, snapshot.Length);
      outliningManager.ExpandAll(span, collapsed => true);
    }
  }
}
