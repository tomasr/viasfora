using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Design;
using Winterdom.Viasfora.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.TextObfuscationOptions)]
  public class TextObfuscationOptions : DialogPage {
    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      VsfSettings.TextObfuscationEnabled = this.TextObfuscationEnabled;
      VsfSettings.TextObfuscationRegexes = Expressions.ListToJson();
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      this.TextObfuscationEnabled = VsfSettings.TextObfuscationEnabled;
      this.Expressions = VsfSettings.TextObfuscationRegexes.ListFromJson<RegexEntry>();
    }

    [LocDisplayName("Enable Obfuscation")]
    [Description("Enables obfuscation of text that matches the configured expressions")]
    [Category("Obfuscation")]
    public bool TextObfuscationEnabled { get; set; }

    [LocDisplayName("Expressions")]
    [Description("Expressions used to define what text to obfuscate")]
    [Category("Obfuscation")]
    [Editor(typeof(System.ComponentModel.Design.CollectionEditor), typeof(UITypeEditor))]
    public List<RegexEntry> Expressions { get; set; }
  }
}