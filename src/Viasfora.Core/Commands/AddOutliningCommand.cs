using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  [Export(typeof(ITextViewCommandHandler))]
  public class AddOutliningCommand : ITextViewCommandHandler {
    public Guid CommandGroup => new Guid(Guids.guidVsfTextEditorCmdSet);
    public int CommandId => PkgCmdIdList.cmdidAddOutlining;
    [Import]
    public IVsfTelemetry Telemetry { get; set; }

    public bool IsEnabled(ITextView view, ref String commandText) {
      return TextEditor.SupportsOutlines(view)
          && !view.Selection.IsEmpty;
    }

    public bool Handle(ITextView view) {
      var selection = view.Selection;
      if ( selection != null ) {
        if ( selection.Mode == TextSelectionMode.Box ) {
          // not supported, ignore for now;
          return false;
        }
        // in many cases, the buffer at the top of the
        // buffer graph will be a projection buffer, which
        // is problematic for the ASPX editor. Instead
        // look for the first, non-projection buffer
        // on the graph and project on it.
        SnapshotSpan? span = TextEditor.MapSelectionToPrimaryBuffer(selection);
        //SnapshotSpan? span = selection.StreamSelectionSpan.SnapshotSpan;
        if ( span != null ) {
          AddOutlining(span.Value.Snapshot.TextBuffer, span.Value);
          var oc = OutliningController.Get(view);
          if ( oc != null ) {
            oc.CollapseRegion(span.Value);
          }
          return true;
        }
      }
      return false;
    }
    private void AddOutlining(ITextBuffer buffer, SnapshotSpan span) {
      var outlines = UserOutliningManager.Get(buffer);
      outlines.Add(span);
      Telemetry.WriteEvent("Add Outlining");
    }
  }
}
