using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.MainOptions)]
  public class MainOptionsPage : DialogPage {
    private MainOptionsControl dialog;

    protected override IWin32Window Window => this.dialog;

    public MainOptionsPage() {
      this.dialog = new MainOptionsControl();
    }

    public override void SaveSettingsToStorage() {
    }

    public override void LoadSettingsFromStorage() {
    }

  }
}
