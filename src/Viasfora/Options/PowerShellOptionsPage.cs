using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.PowerShellOptions)]
  public class PowerShellOptionsPage : DialogPage {
    private ILanguage language = SettingsContext.GetLanguage(Langs.PowerShell);

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      this.language.Settings.ControlFlow = ControlFlowKeywords.ToArray();
      this.language.Settings.Enabled = Enabled;
      this.language.Settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      ControlFlowKeywords = this.language.Settings.ControlFlow.ToList();
      Enabled = this.language.Settings.Enabled;
    }

    [LocDisplayName("Enabled")]
    [Description("Enabled or disables all Viasfora features for this language")]
    public bool Enabled { get; set; }

    [LocDisplayName("Control Flow")]
    [Description("Control Flow keywords to highlight")]
    [Category("PowerShell")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> ControlFlowKeywords { get; set; }
  }
}
