using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.PresentationModeOptions)]
  public class PresentationModeOptionsPage : DialogPage {
    private PresentationModeDialogPage dialog;
    protected override IWin32Window Window {
      get {
        return dialog;
      }
    }

    public PresentationModeOptionsPage() {
      this.dialog = new PresentationModeDialogPage();
    }

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      VsfSettings.PresentationModeDefaultZoomLevel = dialog.DefaultZoom;
      VsfSettings.PresentationModeEnabledZoomLevel = dialog.EnabledZoom;
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      dialog.DefaultZoom = VsfSettings.PresentationModeDefaultZoomLevel;
      dialog.EnabledZoom = VsfSettings.PresentationModeEnabledZoomLevel;
    }
  }
}
