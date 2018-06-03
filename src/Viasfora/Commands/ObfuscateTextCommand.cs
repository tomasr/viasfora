using System;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Compatibility;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Commands {
  public class ObfuscateTextCommand : VsCommand {
    private IVsfTelemetry telemetry;
    public ObfuscateTextCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfViewCmdSet), PkgCmdIdList.cmdidObfuscateText);
      SComponentModel model = new SComponentModel();
      this.telemetry = model.GetService<IVsfTelemetry>();
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      Command.Checked = TextObfuscationState.Enabled;
      Command.Enabled = true;
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      TextObfuscationState.Invert();
      // we'll make do without telemetry for now
      this.telemetry?.WriteEvent("Obfuscate Text");
    }
  }
}
