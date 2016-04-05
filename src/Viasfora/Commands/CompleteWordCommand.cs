using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Text;

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
