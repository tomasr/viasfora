using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Design;
using Winterdom.Viasfora.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.TextObfuscationOptions)]
  public class TextObfuscationOptions : DialogPage {
    private TextObfuscationDialogPage dialog;
    protected override IWin32Window Window {
      get {
        return dialog;
      }
    }

    public TextObfuscationOptions() {
      this.dialog = new TextObfuscationDialogPage();
    }
    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      VsfSettings.TextObfuscationRegexes = dialog.Expressions.ListToJson();
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      this.dialog.Expressions = VsfSettings.TextObfuscationRegexes.ListFromJson<RegexEntry>();
      this.dialog.DataLoaded();
    }
  }
}