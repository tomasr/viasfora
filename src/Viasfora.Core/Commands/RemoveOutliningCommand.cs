using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  [Export(typeof(ITextViewCommandHandler))]
  public class RemoveOutliningCommand : ITextViewCommandHandler {
    public Guid CommandGroup => new Guid(Guids.guidVsfTextEditorCmdSet);
    public int CommandId => PkgCmdIdList.cmdidRemoveOutlining;
    [Import]
    public IVsfTelemetry Telemetry { get; set; }

    public bool IsEnabled(ITextView view, ref String commandText) {
      ITextCaret caret = view.Caret;
      if ( caret == null ) return false;

      var point = TextEditor.MapCaretToPrimaryBuffer(view);

      if ( point != null ) {
        IUserOutlining outlining =
          UserOutliningManager.Get(point.Value.Snapshot.TextBuffer);
        return outlining.IsInOutliningRegion(point.Value);
      }
      return false;
    }

    public bool Handle(ITextView view) {
      ITextCaret caret = view.Caret;

      if ( caret == null ) return false;

      var point = TextEditor.MapCaretToPrimaryBuffer(view);
      //SnapshotPoint? point = caret.Position.BufferPosition;
      if ( point != null ) {
        IUserOutlining outlining = 
          UserOutliningManager.Get(point.Value.Snapshot.TextBuffer);
        outlining.RemoveAt(point.Value);
        Telemetry.WriteEvent("Remove Outlining");
      }
      return true;
    }
  }
}
