using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
      var settings = SettingsContext.GetSettings();
      settings.PresentationModeEnabled = dialog.PMEnabled;
      settings.PresentationModeDefaultZoom = dialog.DefaultZoom;
      settings.PresentationModeEnabledZoom = dialog.EnabledZoom;
      settings.PresentationModeIncludeEnvFonts = dialog.IncludeEnvironmentFonts;
      settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      var settings = SettingsContext.GetSettings();
      dialog.PMEnabled = settings.PresentationModeEnabled;
      dialog.DefaultZoom = settings.PresentationModeDefaultZoom;
      dialog.EnabledZoom = settings.PresentationModeEnabledZoom;
      dialog.IncludeEnvironmentFonts = settings.PresentationModeIncludeEnvFonts;
    }
  }
}
