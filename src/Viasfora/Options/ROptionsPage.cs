using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.ROptions)]
  class ROptionsPage : DialogPage {
    private ILanguage language = SettingsContext.GetLanguage(Langs.R);

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      language.Settings.ControlFlow = ControlFlowKeywords.ToArray();
      language.Settings.Linq = LinqKeywords.ToArray();
      language.Settings.Enabled = Enabled;
      language.Settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      ControlFlowKeywords = language.Settings.ControlFlow.ToList();
      LinqKeywords = language.Settings.Linq.ToList();
      Enabled = language.Settings.Enabled;
    }

    [LocDisplayName("Enabled")]
    [Description("Enabled or disables all Viasfora features for this language")]
    public bool Enabled { get; set; }

    [LocDisplayName("Control Flow")]
    [Description("Control Flow keywords to highlight")]
    [Category("R Language")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> ControlFlowKeywords { get; set; }

    [LocDisplayName("Query")]
    [Description("Query keywords to highlight")]
    [Category("R Language")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> LinqKeywords{ get; set; }
  }
}
