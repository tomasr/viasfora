using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.JScriptOptions)]
  public class JScriptOptionsPage : DialogPage {
    private JScript language = new JScript();

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      language.ControlFlow = ControlFlowKeywords.ToArray();
      language.Linq = LinqKeywords.ToArray();
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      ControlFlowKeywords = language.ControlFlow.ToList();
      LinqKeywords = language.Linq.ToList();
    }

    [LocDisplayName("Control Flow")]
    [Description("Control Flow keywords to highlight")]
    [Category("JavaScript")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> ControlFlowKeywords { get; set; }

    [LocDisplayName("Query")]
    [Description("Query keywords to highlight")]
    [Category("JavaScript")]
    [Editor(Constants.STRING_COLLECTION_EDITOR, typeof(UITypeEditor))]
    [TypeConverter(typeof(Design.StringListConverter))]
    public List<String> LinqKeywords { get; set; }
  }
}
