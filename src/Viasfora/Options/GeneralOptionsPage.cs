using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Winterdom.Viasfora.Rainbow;
using System.Drawing;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.GeneralOptions)]
  public class GeneralOptionsPage : DialogPage {
    private ClassificationList colors;

    public override void SaveSettingsToStorage() {
      var settings = SettingsContext.GetSettings();
      var rainbowSettings = SettingsContext.GetService<IRainbowSettings>();

      settings.CurrentColumnHighlightEnabled = CurrentColumnHighlightEnabled;
      settings.CurrentColumnHighlightStyle = CurrentColumnHighlightStyle;
      settings.HighlightLineWidth = HighlightLineWidth;
      settings.KeywordClassifierEnabled = KeywordClassifierEnabled;
      settings.FlowControlUseItalics = FlowControlUseItalics;
      settings.EscapeSequencesEnabled = EscapeSeqHighlightEnabled;
      settings.DeveloperMarginEnabled = DevMarginEnabled;
      settings.AutoExpandRegions = AutoExpandRegions;
      settings.BoldAsItalicsEnabled = BoldAsItalicsEnabled;
      settings.ModelinesEnabled = ModelinesEnabled;
      settings.ModelinesNumLines = (int)ModelinesNumLines;
      settings.TelemetryEnabled = TelemetryEnabled;
      settings.Save();

      colors.Save();
    }
    public override void LoadSettingsFromStorage() {
      var settings = SettingsContext.GetSettings();

      CurrentColumnHighlightEnabled = settings.CurrentColumnHighlightEnabled;
      CurrentColumnHighlightStyle = settings.CurrentColumnHighlightStyle;
      highlightLineWidth = settings.HighlightLineWidth;
      KeywordClassifierEnabled = settings.KeywordClassifierEnabled;
      FlowControlUseItalics = settings.FlowControlUseItalics;
      EscapeSeqHighlightEnabled = settings.EscapeSequencesEnabled;
      DevMarginEnabled = settings.DeveloperMarginEnabled;
      AutoExpandRegions = settings.AutoExpandRegions;
      BoldAsItalicsEnabled = settings.BoldAsItalicsEnabled;
      ModelinesEnabled = settings.ModelinesEnabled;
      ModelinesNumLines = (uint)settings.ModelinesNumLines;
      TelemetryEnabled = settings.TelemetryEnabled;

      this.colors = new ClassificationList(new ColorStorage(this.Site));
      colors.Load(
        Constants.COLUMN_HIGHLIGHT,
        Constants.KEYWORD_CLASSIF_NAME,
        Constants.LINQ_CLASSIF_NAME,
        Constants.VISIBILITY_CLASSIF_NAME,
        Constants.STRING_ESCAPE_CLASSIF_NAME,
        Constants.FORMAT_SPECIFIER_NAME
        );
    }

    // General Settings
    [LocDisplayName("Enable Telemetry")]
    [Description("Enable sending telemetry about Viasfora usage. Will only take effect after a restart.")]
    [Category("General")]
    public bool TelemetryEnabled { get; set; }

    [LocDisplayName("Enable Developer Margin")]
    [Description("Enables the VS text editor extension developer margin")]
    [Category("General")]
    public bool DevMarginEnabled { get; set; }

    [LocDisplayName("Expand Regions on Open")]
    [Description("Automatically expand collapsible regions when a new text view is opened")]
    [Category("General")]
    public Outlining.AutoExpandMode AutoExpandRegions { get; set; }


    // Text Editor Extensions
    [LocDisplayName("Enable Keyword Classifier")]
    [Description("Enable custom keyword highlighting for C#, CPP and JS")]
    [Category("Keyword Highlight")]
    public bool KeywordClassifierEnabled { get; set; }

    [LocDisplayName("Use italics on Flow Control Keywords")]
    [Description("Use italics on text highlighted by the Keyword Classifier")]
    [Category("Text Editor")]
    public bool FlowControlUseItalics { get; set; }


    [LocDisplayName("Enable 'Bold As Italics'")]
    [Description("Render bold fonts using italics instead")]
    [Category("Text Editor")]
    public bool BoldAsItalicsEnabled { get; set; }

    [LocDisplayName("Highlight Escape Sequences")]
    [Description("Enable highlighting of escape sequences in strings")]
    [Category("Text Editor")]
    public bool EscapeSeqHighlightEnabled { get; set; }

    [LocDisplayName("Flow Control Keywords")]
    [Description("Foreground color used to highlight flow control keywords")]
    [Category("Text Editor")]
    public Color FlowControlForegroundColor {
      get { return colors.Get(Constants.KEYWORD_CLASSIF_NAME, true); }
      set { colors.Set(Constants.KEYWORD_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("Query Keywords")]
    [Description("Foreground color used to highlight LINQ/Query keywords")]
    [Category("Text Editor")]
    public Color LinqForegroundColor {
      get { return colors.Get(Constants.LINQ_CLASSIF_NAME, true); }
      set { colors.Set(Constants.LINQ_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("Visibility Keywords")]
    [Description("Foreground color used to highlight visibility keywords")]
    [Category("Text Editor")]
    public Color VisibilityForegroundColor {
      get { return colors.Get(Constants.VISIBILITY_CLASSIF_NAME, true); }
      set { colors.Set(Constants.VISIBILITY_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("String Escape Sequences")]
    [Description("Foreground color used to highlight escape sequences in strings")]
    [Category("Text Editor")]
    public Color StringEscapeSeqColor {
      get { return colors.Get(Constants.STRING_ESCAPE_CLASSIF_NAME, true); }
      set { colors.Set(Constants.STRING_ESCAPE_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("String Format Specifiers")]
    [Description("Foreground color used to highlight format specifiers in strings")]
    [Category("Text Editor")]
    public Color StringFormatSpecsColor {
      get { return colors.Get(Constants.FORMAT_SPECIFIER_NAME, true); }
      set { colors.Set(Constants.FORMAT_SPECIFIER_NAME, true, value); }
    }


    // current column highlight
    private double highlightLineWidth;
    [LocDisplayName("Highlight Line Width")]
    [Description("Defines the thickness of the current line/column highlight")]
    [Category("Location Tracking")]
    public double HighlightLineWidth {
      get { return this.highlightLineWidth; }
      set {
        if ( value <= 0.0 ) {
          throw new ArgumentOutOfRangeException("Value must be greater than 0");
        }
        this.highlightLineWidth = value;
      }
    }

    [LocDisplayName("Column Highlight")]
    [Description("Enables highlighting the current column in the text editor")]
    [Category("Location Tracking")]
    public bool CurrentColumnHighlightEnabled { get; set; }

    [LocDisplayName("Column Highlight Style")]
    [Description("Controls how the current column highlight is rendered.")]
    [Category("Location Tracking")]
    public ColumnStyle CurrentColumnHighlightStyle { get; set; }

    [LocDisplayName("Column Highlight Foreground")]
    [Description("Foreground color used to highlight the current column")]
    [Category("Location Tracking")]
    public Color ColumnHighlightForeground {
      get { return colors.Get(Constants.COLUMN_HIGHLIGHT, true); }
      set { colors.Set(Constants.COLUMN_HIGHLIGHT, true, value); }
    }

    [LocDisplayName("Column Highlight Background")]
    [Description("Background color used to highlight the current column")]
    [Category("Location Tracking")]
    public Color ColumnHighlightBackground {
      get { return colors.Get(Constants.COLUMN_HIGHLIGHT, false); }
      set { colors.Set(Constants.COLUMN_HIGHLIGHT, false, value); }
    }




    // Modelines Configuration
    [LocDisplayName("Enable Modelines Support")]
    [Description("Enables the use of Vim-style modelines to configure the text editor")]
    [Category("Modelines")]
    public bool ModelinesEnabled { get; set; }

    [LocDisplayName("Lines to Check")]
    [Description("Number of lines to check for modeline commands")]
    [Category("Modelines")]
    public uint ModelinesNumLines {get; set; }


  }
}
