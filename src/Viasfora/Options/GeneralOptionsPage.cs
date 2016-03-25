using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Design;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.GeneralOptions)]
  public class GeneralOptionsPage : DialogPage {

    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      var settings = SettingsContext.GetSettings();
      settings.CurrentLineHighlightEnabled = CurrentLineHighlightEnabled;
      settings.CurrentColumnHighlightEnabled = CurrentColumnHighlightEnabled;
      settings.HighlightLineWidth = this.HighlightLineWidth;
      settings.KeywordClassifierEnabled = KeywordClassifierEnabled;
      settings.FlowControlUseItalics = FlowControlUseItalics;
      settings.EscapeSequencesEnabled = EscapeSeqHighlightEnabled;
      settings.RainbowDepth = RainbowDepth;
      settings.RainbowTagsEnabled = RainbowTagsEnabled;
      settings.RainbowToolTipsEnabled = RainbowToolTipsEnabled;
      settings.RainbowHighlightMode = RainbowHighlightMode;
      settings.DeveloperMarginEnabled = DevMarginEnabled;
      settings.AutoExpandRegions = AutoExpandRegions;
      settings.BoldAsItalicsEnabled = BoldAsItalicsEnabled;
      settings.ModelinesEnabled = ModelinesEnabled;
      settings.ModelinesNumLines = (int)ModelinesNumLines;
      settings.XmlnsPrefixEnabled = XmlnsPrefixHighlightEnabled;
      settings.XmlCloseTagEnabled = XmlCloseTagHighlightEnabled;
      settings.XmlMatchTagsEnabled = XmlMatchTagsEnabled;
      settings.TelemetryEnabled = TelemetryEnabled;
      settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      var settings = SettingsContext.GetSettings();
      CurrentLineHighlightEnabled = settings.CurrentLineHighlightEnabled;
      CurrentColumnHighlightEnabled = settings.CurrentColumnHighlightEnabled;
      highlightLineWidth = settings.HighlightLineWidth;
      KeywordClassifierEnabled = settings.KeywordClassifierEnabled;
      FlowControlUseItalics = settings.FlowControlUseItalics;
      EscapeSeqHighlightEnabled = settings.EscapeSequencesEnabled;
      RainbowDepth = settings.RainbowDepth;
      RainbowTagsEnabled = settings.RainbowTagsEnabled;
      RainbowHighlightMode = settings.RainbowHighlightMode;
      RainbowToolTipsEnabled = settings.RainbowToolTipsEnabled;
      DevMarginEnabled = settings.DeveloperMarginEnabled;
      AutoExpandRegions = settings.AutoExpandRegions;
      BoldAsItalicsEnabled = settings.BoldAsItalicsEnabled;
      ModelinesEnabled = settings.ModelinesEnabled;
      ModelinesNumLines = (uint)settings.ModelinesNumLines;
      XmlnsPrefixHighlightEnabled = settings.XmlnsPrefixEnabled;
      XmlCloseTagHighlightEnabled = settings.XmlCloseTagEnabled;
      XmlMatchTagsEnabled = settings.XmlMatchTagsEnabled;
      TelemetryEnabled = settings.TelemetryEnabled;
    }

    // General Settings
    [LocDisplayName("Enable Telemetry")]
    [Description("Enable sending telemetry about Viasfora usage. Will only take effect after a restart.")]
    [Category("General")]
    public bool TelemetryEnabled { get; set; }

    // Text Editor Extensions
    [LocDisplayName("Enable Keyword Classifier")]
    [Description("Enable custom keyword highlighting for C#, CPP and JS")]
    [Category("Text Editor")]
    public bool KeywordClassifierEnabled { get; set; }
    [LocDisplayName("Use italics on Flow Control Keywords")]
    [Description("Use italics on text highlighted by the Keyword Classifier")]
    [Category("Text Editor")]
    public bool FlowControlUseItalics { get; set; }


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
    [Category("Rainbow Braces")]
    public bool RainbowTagsEnabled { get; set; }

    private int rainbowDepth;
    [LocDisplayName("Rainbow Depth")]
    [Description("Controls how many different colors are used to render rainbow braces")]
    [Category("Rainbow Braces")]
    public int RainbowDepth {
      get { return this.rainbowDepth; }
      set {
        if ( value <= 0 || value > Constants.MAX_RAINBOW_DEPTH ) {
          throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
        }
        this.rainbowDepth = value;
      }
    }

    [LocDisplayName("Rainbow Highlight Mode")]
    [Description("Controls how the caret position is used to identify braces to highlight.")]
    [Category("Rainbow Braces")]
    public RainbowHighlightMode RainbowHighlightMode { get; set; }

    [LocDisplayName("Enable Rainbow ToolTips")]
    [Description("Show a tooltip highlighting matching braces when you hover the mouse over a rainbow brace")]
    [Category("Rainbow Braces")]
    public bool RainbowToolTipsEnabled { get; set; }


    [LocDisplayName("Enable Developer Margin")]
    [Description("Enables the VS text editor extension developer margin")]
    [Category("Text Editor")]
    public bool DevMarginEnabled { get; set; }


    [LocDisplayName("Expand Regions on Open")]
    [Description("Automatically expand collapsible regions when a new text view is opened")]
    [Category("Text Editor")]
    public Outlining.AutoExpandMode AutoExpandRegions { get; set; }

    [LocDisplayName("Enable 'Bold As Italics'")]
    [Description("Render bold fonts using italics instead")]
    [Category("Text Editor")]
    public bool BoldAsItalicsEnabled { get; set; }


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
