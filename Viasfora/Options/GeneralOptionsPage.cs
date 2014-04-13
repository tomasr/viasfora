using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Design;
using System.Drawing.Design;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.GeneralOptions)]
  public class GeneralOptionsPage : DialogPage {

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      VsfSettings.CurrentLineHighlightEnabled = CurrentLineHighlightEnabled;
      VsfSettings.CurrentColumnHighlightEnabled = CurrentColumnHighlightEnabled;
      VsfSettings.HighlightLineWidth = this.HighlightLineWidth;
      VsfSettings.KeywordClassifierEnabled = KeywordClassifierEnabled;
      VsfSettings.EscapeSeqHighlightEnabled = EscapeSeqHighlightEnabled;
      VsfSettings.RainbowTagsEnabled = RainbowTagsEnabled;
      VsfSettings.DevMarginEnabled = DevMarginEnabled;
      VsfSettings.TextCompletionEnabled = TextCompletionEnabled;
      VsfSettings.AutoExpandRegions = AutoExpandRegions;
      VsfSettings.ModelinesEnabled = ModelinesEnabled;
      VsfSettings.ModelinesNumLines = (int)ModelinesNumLines;
      VsfSettings.XmlnsPrefixHighlightEnabled = XmlnsPrefixHighlightEnabled;
      VsfSettings.XmlCloseTagHighlightEnabled = XmlCloseTagHighlightEnabled;
      VsfSettings.XmlMatchTagsEnabled = XmlMatchTagsEnabled;
      VsfSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      CurrentLineHighlightEnabled = VsfSettings.CurrentLineHighlightEnabled;
      CurrentColumnHighlightEnabled = VsfSettings.CurrentColumnHighlightEnabled;
      highlightLineWidth = VsfSettings.HighlightLineWidth;
      KeywordClassifierEnabled = VsfSettings.KeywordClassifierEnabled;
      EscapeSeqHighlightEnabled = VsfSettings.EscapeSeqHighlightEnabled;
      RainbowTagsEnabled = VsfSettings.RainbowTagsEnabled;
      DevMarginEnabled = VsfSettings.DevMarginEnabled;
      TextCompletionEnabled = VsfSettings.TextCompletionEnabled;
      AutoExpandRegions = VsfSettings.AutoExpandRegions;
      ModelinesEnabled = VsfSettings.ModelinesEnabled;
      ModelinesNumLines = (uint)VsfSettings.ModelinesNumLines;
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

    [LocDisplayName("Highlight Current Column")]
    [Description("Enables highlighting the current column in the text editor")]
    [Category("Text Editor")]
    public bool CurrentColumnHighlightEnabled { get; set; }

    private double highlightLineWidth;
    [LocDisplayName("Highlight Line Width")]
    [Description("Defines the thickness of the current line/column highlight")]
    [Category("Text Editor")]
    public double HighlightLineWidth {
      get { return this.highlightLineWidth; }
      set {
        if ( value <= 0.0 ) {
          throw new ArgumentOutOfRangeException("Value must be greater than 0");
        }
        this.highlightLineWidth = value;
      }
    }

    [LocDisplayName("Enable Rainbow Braces")]
    [Description("Highlight matching braces using colors based on nesting")]
    [Category("Text Editor")]
    public bool RainbowTagsEnabled { get; set; }

    [LocDisplayName("Enable Developer Margin")]
    [Description("Enables the VS text editor extension developer margin")]
    [Category("Text Editor")]
    public bool DevMarginEnabled { get; set; }

    [LocDisplayName("Enable Plain-Text Completion")]
    [Description("Enables auto-completion based on the plain text of the current document")]
    [Category("Text Editor")]
    public bool TextCompletionEnabled { get; set; }

    [LocDisplayName("Expand Regions on Open")]
    [Description("Automatically expand collapsible regions when a new text view is opened")]
    [Category("Text Editor")]
    public bool AutoExpandRegions { get; set; }


    // Modelines Configuration
    [LocDisplayName("Enable Modelines Support")]
    [Description("Enables the use of Vim-style modelines to configure the text editor")]
    [Category("Modelines")]
    public bool ModelinesEnabled { get; set; }

    [LocDisplayName("Lines to Check")]
    [Description("Number of lines to check for modeline commands")]
    [Category("Modelines")]
    public uint ModelinesNumLines {get; set; }


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
