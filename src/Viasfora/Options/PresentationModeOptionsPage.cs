using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.PresentationModeOptions)]
  public class PresentationModeOptionsPage : DialogPage {
    private PresentationModeDialogPage dialog;
    protected override IWin32Window Window => this.dialog;

    public PresentationModeOptionsPage() {
      this.dialog = new PresentationModeDialogPage();
    }

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      var settings = SettingsContext.GetSettings();
      settings.PresentationModeEnabled = this.dialog.PMEnabled;
      settings.PresentationModeDefaultZoom = this.dialog.DefaultZoom;
      settings.PresentationModeEnabledZoom = this.dialog.EnabledZoom;
      settings.PresentationModeIncludeEnvFonts = this.dialog.IncludeEnvironmentFonts;
      settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      var settings = SettingsContext.GetSettings();
      this.dialog.PMEnabled = settings.PresentationModeEnabled;
      this.dialog.DefaultZoom = settings.PresentationModeDefaultZoom;
      this.dialog.EnabledZoom = settings.PresentationModeEnabledZoom;
      this.dialog.IncludeEnvironmentFonts = settings.PresentationModeIncludeEnvFonts;
    }
  }
}
