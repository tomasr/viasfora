using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.PowerShellOptions)]
  public class PowerShellOptionsPage : DialogPage {
    private ILanguage language = SettingsContext.GetLanguage(Constants.PowerShell);

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      language.Settings.ControlFlow = ControlFlowKeywords.ToArray();
      language.Settings.Enabled = Enabled;
      language.Settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      ControlFlowKeywords = language.Settings.ControlFlow.ToList();
      Enabled = language.Settings.Enabled;
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
