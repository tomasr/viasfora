using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.VBOptions)]
  public class VBOptionsPage : DialogPage {
    private ILanguage language = SettingsContext.GetLanguage(Langs.VB);

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      this.language.Settings.ControlFlow = ControlFlowKeywords.ToArray();
      this.language.Settings.Linq = LinqKeywords.ToArray();
      this.language.Settings.Visibility = VisibilityKeywords.ToArray();
      this.language.Settings.Enabled = Enabled;
      this.language.Settings.ReduceOpacityForArgumentValidation = ReduceOpacityForArgumentValidation;
      this.language.Settings.ReduceOpacityForMethodOverloads = ReduceOpacityForMethodOverloads;
      this.language.Settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      ControlFlowKeywords = this.language.Settings.ControlFlow.ToList();
      LinqKeywords = this.language.Settings.Linq.ToList();
      VisibilityKeywords = this.language.Settings.Visibility.ToList();
      Enabled = this.language.Settings.Enabled;
      ReduceOpacityForArgumentValidation = this.language.Settings.ReduceOpacityForArgumentValidation;
      ReduceOpacityForMethodOverloads = this.language.Settings.ReduceOpacityForMethodOverloads;
    }

    [LocDisplayName("Enabled")]
    [Description("Enabled or disables all Viasfora features for this language")]
    public bool Enabled { get; set; }

    [LocDisplayName("Control Flow")]
    [Description("Control Flow keywords to highlight")]
    [Category("Basic")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> ControlFlowKeywords { get; set; }

    [LocDisplayName("Visibility")]
    [Description("Visibility keywords to highlight")]
    [Category("Basic")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> VisibilityKeywords { get; set; }

    [LocDisplayName("LINQ")]
    [Description("LINQ keywords to highlight")]
    [Category("Basic")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> LinqKeywords { get; set; }

    [Category("Reduce Opacity")]
    [LocDisplayName("ArgumentValidation")]
    [Description("Reduce opacity for argument validation")]
    public bool ReduceOpacityForArgumentValidation { get; set; }

    [Category("Reduce Opacity")]
    [LocDisplayName("MethodOverloads")]
    [Description("Reduce opacity for method overloads")]
    public bool ReduceOpacityForMethodOverloads { get; set; }
  }
}
