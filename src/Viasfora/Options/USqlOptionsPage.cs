using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.USqlOptions)]
  public class USqlOptionsPage : DialogPage {
    private ILanguage language = SettingsContext.GetLanguage(Langs.USql);

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      language.Settings.Visibility = VisibilityKeywords.ToArray();
      language.Settings.Linq = LinqKeywords.ToArray();
      language.Settings.Enabled = Enabled;
      language.Settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      VisibilityKeywords = language.Settings.Visibility.ToList();
      LinqKeywords = language.Settings.Linq.ToList();
      Enabled = language.Settings.Enabled;
    }

    [LocDisplayName("Enabled")]
    [Description("Enabled or disables all Viasfora features for this language")]
    public bool Enabled { get; set; }

    [LocDisplayName("Visibility")]
    [Description("Visibility keywords to highlight")]
    [Category("U-SQL")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> VisibilityKeywords { get; set; }

    [LocDisplayName("Statement")]
    [Description("Statement keywords to highlight")]
    [Category("U-SQL")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> LinqKeywords { get; set; }
  }
}
