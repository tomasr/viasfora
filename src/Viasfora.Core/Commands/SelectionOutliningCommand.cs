using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  [Export(typeof(ITextViewCommandHandler))]
  public class SelectionOutliningCommand : ITextViewCommandHandler {
    public Guid CommandGroup => new Guid(Guids.guidVsfTextEditorCmdSet);
    public int CommandId => PkgCmdIdList.cmdidSelectionOutlining;
    [Import]
    public IVsfTelemetry Telemetry { get; set; }

    public bool IsEnabled(ITextView view, ref String commandText) {
      if ( TextEditor.SupportsOutlines(view) ) {
        if ( HasFeatureOutlines(view) ) {
          commandText = "Remove Selection Outline";
          return true;
        } else if ( !view.Selection.IsEmpty ) {
          commandText = "Outline Selection";
          return true;
        }
      }
      return false;
    }

    public bool Handle(ITextView view) {
      if ( HasFeatureOutlines(view) ) {
        ClearOutlines(view);
        return true;
      }
      var selection = view.Selection;
      SnapshotSpan? span = selection.StreamSelectionSpan.SnapshotSpan;
      if ( span.HasValue ) {
        AddOutlining(span.Value.Snapshot.TextBuffer, span.Value);
        CollapseOutlines(selection.TextView);
        Telemetry.WriteEvent("Outline Selection");
      }
      return true;
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
    private void ClearOutlines(ITextView view) {
      var controller = OutliningController.Get(view);
      if ( controller != null ) {
        controller.RemoveSelectionRegions();
      }
    }
    private bool HasFeatureOutlines(ITextView view) {
      var outlines = SelectionOutliningManager.Get(view.TextBuffer);
      return outlines != null && outlines.HasUserOutlines();
    }
  }
}
