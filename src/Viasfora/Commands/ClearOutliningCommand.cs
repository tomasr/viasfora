using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  public class ClearOutliningCommand : VsCommand {
    public ClearOutliningCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidClearOutlining);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      var outlining = GetOutlining(out ITextBuffer buffer);
      Command.Enabled = outlining != null && outlining.HasUserOutlines();
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      var outlining = GetOutlining(out ITextBuffer buffer);
      if ( outlining != null ) {
        outlining.RemoveAll(buffer.CurrentSnapshot);
        //Telemetry.WriteEvent("Clear Outlining");
      }
    }

    private IUserOutlining GetOutlining(out ITextBuffer buffer) {
      buffer = null;
      ITextView view = TextEditor.GetCurrentView();
      if ( view != null ) {
        buffer = TextEditor.GetPrimaryBuffer(view);
        //buffer = view.TextBuffer;
        return UserOutliningManager.Get(buffer);
      }
      return null;
    }
  }
}
