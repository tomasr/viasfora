using System;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Compatibility;

namespace Winterdom.Viasfora.Commands {
  public class PresentationModeCommand : VsCommand {
    private IVsfSettings settings = SettingsContext.GetSettings();
    private IPresentationModeState state;
    public PresentationModeCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {
      var model = new SComponentModel();
      this.state = model.GetService<IPresentationModeState>();
      Initialize(new Guid(Guids.guidVsfViewCmdSet), PkgCmdIdList.cmdidPresentationMode);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      Command.Checked = state.PresentationModeTurnedOn;
      Command.Enabled = settings.PresentationModeEnabled;
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      state.TogglePresentationMode();
    }
  }
}
