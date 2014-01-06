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
      VsfSettings.PresentationModeEnabled = dialog.PMEnabled;
      VsfSettings.PresentationModeDefaultZoomLevel = dialog.DefaultZoom;
      VsfSettings.PresentationModeEnabledZoomLevel = dialog.EnabledZoom;
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      dialog.PMEnabled = VsfSettings.PresentationModeEnabled;
      dialog.DefaultZoom = VsfSettings.PresentationModeDefaultZoomLevel;
      dialog.EnabledZoom = VsfSettings.PresentationModeEnabledZoomLevel;
    }
  }
}
