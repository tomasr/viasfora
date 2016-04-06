using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  public class SelectionOutliningCommand : VsCommand {
    public SelectionOutliningCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidSelectionOutlining);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      var view = TextEditor.GetCurrentView();
      if ( view != null && TextEditor.SupportsOutlines(view) ) {
        if ( HasFeatureOutlines() ) {
          Command.Text = "Remove Selection Outline";
          Command.Enabled = true;
          return;
        } else if ( TextEditor.GetCurrentSelection() != null ) {
          Command.Text = "Outline Selection";
          Command.Enabled = true;
          return;
        }
      }
      Command.Enabled = false;
    }

    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      if ( HasFeatureOutlines() ) {
        ClearOutlines();
        return;
      }
      ITextSelection selection = TextEditor.GetCurrentSelection();
      if ( selection == null ) {
        return;
      }
      ITextView view = selection.TextView;
      SnapshotSpan? span = selection.StreamSelectionSpan.SnapshotSpan;
      if ( span.HasValue ) {
        AddOutlining(span.Value.Snapshot.TextBuffer, span.Value);
        CollapseOutlines(selection.TextView);
        Telemetry.WriteEvent("Outline Selection");
      }
    }

    private void AddOutlining(ITextBuffer buffer, SnapshotSpan span) {
      var outlines = SelectionOutliningManager.Get(buffer);
      if ( outlines != null ) {
        outlines.CreateRegionsAround(span);
      }
    }
    private void CollapseOutlines(ITextView textView) {
      var controller = OutliningController.Get(textView);
      if ( controller != null ) {
        controller.CollapseSelectionRegions();
      }
    }
    private void ClearOutlines() {
      var view = TextEditor.GetCurrentView();
      var controller = OutliningController.Get(view);
      if ( controller != null ) {
        controller.RemoveSelectionRegions();
      }
    }
    private bool HasFeatureOutlines() {
      var view = TextEditor.GetCurrentView();
      var outlines = SelectionOutliningManager.Get(view.TextBuffer);
      return outlines != null && outlines.HasUserOutlines();
    }

  }
}
