using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Commands {
  public class PresentationModeCommand : VsCommand {
    private IVsfSettings settings = SettingsContext.GetSettings();
    public PresentationModeCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfViewCmdSet), PkgCmdIdList.cmdidPresentationMode);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      Command.Checked = VsfPackage.PresentationModeTurnedOn;
      Command.Enabled = settings.PresentationModeEnabled;
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      VsfPackage.PresentationModeTurnedOn = !VsfPackage.PresentationModeTurnedOn;
      if ( VsfPackage.PresentationModeChanged != null ) {
        VsfPackage.PresentationModeChanged(this, EventArgs.Empty);
      }
    }
  }
}
