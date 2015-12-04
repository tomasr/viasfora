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
      var settings = SettingsContext.GetSettings();
      settings.PresentationModeEnabled = dialog.PMEnabled;
      settings.PresentationModeDefaultZoomLevel = dialog.DefaultZoom;
      settings.PresentationModeEnabledZoomLevel = dialog.EnabledZoom;
      settings.PresentationModeIncludeEnvironmentFonts = dialog.IncludeEnvironmentFonts;
      settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      var settings = SettingsContext.GetSettings();
      dialog.PMEnabled = settings.PresentationModeEnabled;
      dialog.DefaultZoom = settings.PresentationModeDefaultZoomLevel;
      dialog.EnabledZoom = settings.PresentationModeEnabledZoomLevel;
      dialog.IncludeEnvironmentFonts = settings.PresentationModeIncludeEnvironmentFonts;
    }
  }
}
