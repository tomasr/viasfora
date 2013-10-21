using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Options {
  public class GeneralOptionsPage : DialogPage {
    public GeneralOptionsPage() {
    }

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      VsfSettings.CurrentLineHighlightEnabled = CurrentLineHighlightEnabled;
      VsfSettings.KeywordClassifierEnabled = KeywordClassifierEnabled;
      VsfSettings.XmlExtensionsEnabled = XmlExtensionsEnabled;
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      CurrentLineHighlightEnabled = VsfSettings.CurrentLineHighlightEnabled;
      KeywordClassifierEnabled = VsfSettings.KeywordClassifierEnabled;
      XmlExtensionsEnabled = VsfSettings.XmlExtensionsEnabled;
    }

    [LocDisplayName("Enable Keyword Classifier")]
    [Description("Enable custom keyword highlighting for C#, CPP and JS")]
    [Category("Text Editor")]
    public bool KeywordClassifierEnabled { get; set; }

    [LocDisplayName("Enable Xml Extensions")]
    [Description("Enable XML editor extensions")]
    [Category("XML Editor")]
    public bool XmlExtensionsEnabled { get; set; }

    [LocDisplayName("Highlight Current Line")]
    [Description("Enables highlighting the current line in the text editor")]
    [Category("Text Editor")]
    public bool CurrentLineHighlightEnabled { get; set; }
  }
}
