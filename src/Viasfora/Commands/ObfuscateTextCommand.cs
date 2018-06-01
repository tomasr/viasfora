using System;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Commands {
  public class ObfuscateTextCommand : VsCommand {
    public ObfuscateTextCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfViewCmdSet), PkgCmdIdList.cmdidObfuscateText);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      Command.Checked = TextObfuscationState.Enabled;
      Command.Enabled = true;
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      TextObfuscationState.Invert();
      //Telemetry.WriteEvent("Obfuscate Text");
    }
  }
}
