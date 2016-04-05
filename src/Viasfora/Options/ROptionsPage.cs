using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.ROptions)]
  class ROptionsPage : DialogPage {
    private ILanguage language = SettingsContext.GetLanguage(Constants.R);

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      language.ControlFlow = ControlFlowKeywords.ToArray();
      language.Linq = LinqKeywords.ToArray();
      language.Enabled = Enabled;
      var settings = SettingsContext.GetSettings();
      settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      ControlFlowKeywords = language.ControlFlow.ToList();
      LinqKeywords = language.Linq.ToList();
      Enabled = language.Enabled;
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
