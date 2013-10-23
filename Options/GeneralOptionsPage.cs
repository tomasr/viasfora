using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.GeneralOptions)]
  public class GeneralOptionsPage : DialogPage {

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      VsfSettings.CurrentLineHighlightEnabled = CurrentLineHighlightEnabled;
      VsfSettings.KeywordClassifierEnabled = KeywordClassifierEnabled;
      VsfSettings.EscapeSeqHighlightEnabled = EscapeSeqHighlightEnabled;
      VsfSettings.XmlnsPrefixHighlightEnabled = XmlnsPrefixHighlightEnabled;
      VsfSettings.XmlCloseTagHighlightEnabled = XmlCloseTagHighlightEnabled;
      VsfSettings.XmlMatchTagsEnabled = XmlMatchTagsEnabled;
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      CurrentLineHighlightEnabled = VsfSettings.CurrentLineHighlightEnabled;
      KeywordClassifierEnabled = VsfSettings.KeywordClassifierEnabled;
      EscapeSeqHighlightEnabled = VsfSettings.EscapeSeqHighlightEnabled;
      XmlnsPrefixHighlightEnabled = VsfSettings.XmlnsPrefixHighlightEnabled;
      XmlCloseTagHighlightEnabled = VsfSettings.XmlCloseTagHighlightEnabled;
      XmlMatchTagsEnabled = VsfSettings.XmlMatchTagsEnabled;
    }

    // Text Editor Extensions
    [LocDisplayName("Enable Keyword Classifier")]
    [Description("Enable custom keyword highlighting for C#, CPP and JS")]
    [Category("Text Editor")]
    public bool KeywordClassifierEnabled { get; set; }

    [LocDisplayName("Highlight Escape Sequences")]
    [Description("Enable highlighting of escape sequences in strings")]
    [Category("Text Editor")]
    public bool EscapeSeqHighlightEnabled { get; set; }

    [LocDisplayName("Highlight Current Line")]
    [Description("Enables highlighting the current line in the text editor")]
    [Category("Text Editor")]
    public bool CurrentLineHighlightEnabled { get; set; }


    // XML Editor Extensions
    [LocDisplayName("Highlight XML Namespace Prefix")]
    [Description("Enables highlighting XML Namespace prefixes")]
    [Category("XML Editor")]
    public bool XmlnsPrefixHighlightEnabled { get; set; }

    [LocDisplayName("Highlight XML Closing Tags")]
    [Description("Enables highlighting XML closing element tags")]
    [Category("XML Editor")]
    public bool XmlCloseTagHighlightEnabled { get; set; }

    [LocDisplayName("Match Element Tags")]
    [Description("Enables highlighting XML element opening/closing tags")]
    [Category("XML Editor")]
    public bool XmlMatchTagsEnabled { get; set; }
  }
}
