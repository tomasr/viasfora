using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Commands {
  public abstract class VsCommand {
    public VsfPackage Package { get; private set; }
    public OleMenuCommandService CommandService { get; private set; }
    public OleMenuCommand Command { get; private set; }

    public VsCommand(VsfPackage package, OleMenuCommandService omcs) {
      this.CommandService = omcs;
      this.Package = package;
    }

    protected void Initialize(Guid menuGroup, int commandId) {
      var cmdId = new CommandID(menuGroup, commandId);
      Command = new OleMenuCommand(this.OnInvoke, cmdId);
      Command.BeforeQueryStatus += OnBeforeQueryStatus;
      CommandService.AddCommand(Command);
    }

    protected virtual void OnBeforeQueryStatus(object sender, EventArgs e) {
    }
    protected virtual void OnInvoke(object sender, EventArgs e) {
    }
  }
}
