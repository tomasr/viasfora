using System;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Commands {
  public class CompleteWordCommand : VsCommand {
    public CompleteWordCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidCompleteWord);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      var view = TextEditor.GetCurrentView();
      if ( view == null ) {
        Command.Enabled = false;
      }
    }
  }
}
