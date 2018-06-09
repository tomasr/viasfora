using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  [Export(typeof(ITextViewCommandHandler))]
  public class ClearOutliningCommand : ITextViewCommandHandler {
    public Guid CommandGroup => new Guid(Guids.guidVsfTextEditorCmdSet);
    public int CommandId => PkgCmdIdList.cmdidClearOutlining;
    [Import]
    public IVsfTelemetry Telemetry { get; set; }

    public bool IsEnabled(ITextView view, ref String commandText) {
      var outlining = GetOutlining(view, out ITextBuffer buffer);
      return outlining != null && outlining.HasUserOutlines();
    }

    public bool Handle(ITextView view) {
      var outlining = GetOutlining(view, out ITextBuffer buffer);
      outlining?.RemoveAll(buffer.CurrentSnapshot);
      Telemetry.WriteEvent("Clear Outlining");
      return true;
    }

    private IUserOutlining GetOutlining(ITextView view, out ITextBuffer buffer) {
      buffer = TextEditor.GetPrimaryBuffer(view);
      return UserOutliningManager.Get(buffer);
    }
  }
}
