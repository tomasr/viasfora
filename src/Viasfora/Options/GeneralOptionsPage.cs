using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Drawing;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.GeneralOptions)]
  public class GeneralOptionsPage : DialogPage {
    private ClassificationList colors;

    public override void SaveSettingsToStorage() {
      ThreadHelper.ThrowIfNotOnUIThread();
      var settings = SettingsContext.GetSettings();

      settings.CurrentLineHighlightEnabled = CurrentLineHighlightEnabled;
      settings.CurrentColumnHighlightEnabled = CurrentColumnHighlightEnabled;
      settings.CurrentColumnHighlightStyle = CurrentColumnHighlightStyle;
      settings.HighlightLineWidth = HighlightLineWidth;
      settings.KeywordClassifierEnabled = KeywordClassifierEnabled;
      settings.FlowControlKeywordsEnabled = FlowControlKeywordsEnabled;
      settings.VisibilityKeywordsEnabled = VisibilityKeywordsEnabled;
      settings.QueryKeywordsEnabled = QueryKeywordsEnabled;
      settings.FlowControlUseItalics = FlowControlUseItalics;
      settings.EscapeSequencesEnabled = EscapeSeqHighlightEnabled;
      settings.DeveloperMarginEnabled = DevMarginEnabled;
      settings.AutoExpandRegions = AutoExpandRegions;
      settings.BoldAsItalicsEnabled = BoldAsItalicsEnabled;
      settings.ModelinesEnabled = ModelinesEnabled;
      settings.ModelinesNumLines = (int)ModelinesNumLines;
      settings.TelemetryEnabled = TelemetryEnabled;
      settings.Save();

      this.colors.Save();
    }
    public override void LoadSettingsFromStorage() {
      ThreadHelper.ThrowIfNotOnUIThread();
      var settings = SettingsContext.GetSettings();

      CurrentLineHighlightEnabled = settings.CurrentLineHighlightEnabled;
      CurrentColumnHighlightEnabled = settings.CurrentColumnHighlightEnabled;
      CurrentColumnHighlightStyle = settings.CurrentColumnHighlightStyle;
      this.highlightLineWidth = settings.HighlightLineWidth;
      KeywordClassifierEnabled = settings.KeywordClassifierEnabled;
      FlowControlKeywordsEnabled = settings.FlowControlKeywordsEnabled;
      VisibilityKeywordsEnabled = settings.VisibilityKeywordsEnabled;
      QueryKeywordsEnabled = settings.QueryKeywordsEnabled;
      FlowControlUseItalics = settings.FlowControlUseItalics;
      EscapeSeqHighlightEnabled = settings.EscapeSequencesEnabled;
      DevMarginEnabled = settings.DeveloperMarginEnabled;
      AutoExpandRegions = settings.AutoExpandRegions;
      BoldAsItalicsEnabled = settings.BoldAsItalicsEnabled;
      ModelinesEnabled = settings.ModelinesEnabled;
      ModelinesNumLines = (uint)settings.ModelinesNumLines;
      TelemetryEnabled = settings.TelemetryEnabled;

      this.colors = new ClassificationList(new ColorStorage(this.Site));
      this.colors.Load(
        Constants.LINE_HIGHLIGHT,
        Constants.COLUMN_HIGHLIGHT,
        Constants.FLOW_CONTROL_CLASSIF_NAME,
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
    [Description("Enable custom keyword highlighting for all languages")]
    [Category("Keyword Highlight")]
    public bool KeywordClassifierEnabled { get; set; }

    [LocDisplayName("Enable Flow Control Keywords")]
    [Description("Enable custom keyword highlighting for all languages")]
    [Category("Keyword Highlight")]
    public bool FlowControlKeywordsEnabled { get; set; }

    [LocDisplayName("Enable Visibility Keywords")]
    [Description("Enable custom keyword highlighting for all languages")]
    [Category("Keyword Highlight")]
    public bool VisibilityKeywordsEnabled { get; set; }

    [LocDisplayName("Enable Query Keywords")]
    [Description("Enable custom keyword highlighting for all languages")]
    [Category("Keyword Highlight")]
    public bool QueryKeywordsEnabled { get; set; }

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
      get { return this.colors.Get(Constants.FLOW_CONTROL_CLASSIF_NAME, true); }
      set { this.colors.Set(Constants.FLOW_CONTROL_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("Query Keywords")]
    [Description("Foreground color used to highlight LINQ/Query keywords")]
    [Category("Text Editor")]
    public Color LinqForegroundColor {
      get { return this.colors.Get(Constants.LINQ_CLASSIF_NAME, true); }
      set { this.colors.Set(Constants.LINQ_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("Visibility Keywords")]
    [Description("Foreground color used to highlight visibility keywords")]
    [Category("Text Editor")]
    public Color VisibilityForegroundColor {
      get { return this.colors.Get(Constants.VISIBILITY_CLASSIF_NAME, true); }
      set { this.colors.Set(Constants.VISIBILITY_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("String Escape Sequences")]
    [Description("Foreground color used to highlight escape sequences in strings")]
    [Category("Text Editor")]
    public Color StringEscapeSeqColor {
      get { return this.colors.Get(Constants.STRING_ESCAPE_CLASSIF_NAME, true); }
      set { this.colors.Set(Constants.STRING_ESCAPE_CLASSIF_NAME, true, value); }
    }
    [LocDisplayName("String Format Specifiers")]
    [Description("Foreground color used to highlight format specifiers in strings")]
    [Category("Text Editor")]
    public Color StringFormatSpecsColor {
      get { return this.colors.Get(Constants.FORMAT_SPECIFIER_NAME, true); }
      set { this.colors.Set(Constants.FORMAT_SPECIFIER_NAME, true, value); }
    }

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

    // current line highlight
    [LocDisplayName("Line Highlight")]
    [Description("Enables highlighting the current line in the text editor")]
    [Category("Location Tracking")]
    public bool CurrentLineHighlightEnabled { get; set; }

    [LocDisplayName("Line Highlight Foreground")]
    [Description("Foreground color used to highlight the current line")]
    [Category("Location Tracking")]
    public Color LineHighlightForeground {
      get { return this.colors.Get(Constants.LINE_HIGHLIGHT, true); }
      set { this.colors.Set(Constants.LINE_HIGHLIGHT, true, value); }
    }

    [LocDisplayName("Line Highlight Background")]
    [Description("Background color used to highlight the current line")]
    [Category("Location Tracking")]
    public Color LineHighlightBackground {
      get { return this.colors.Get(Constants.LINE_HIGHLIGHT, false); }
      set { this.colors.Set(Constants.LINE_HIGHLIGHT, false, value); }
    }

    // current column highlight
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
      get { return this.colors.Get(Constants.COLUMN_HIGHLIGHT, true); }
      set { this.colors.Set(Constants.COLUMN_HIGHLIGHT, true, value); }
    }

    [LocDisplayName("Column Highlight Background")]
    [Description("Background color used to highlight the current column")]
    [Category("Location Tracking")]
    public Color ColumnHighlightBackground {
      get { return this.colors.Get(Constants.COLUMN_HIGHLIGHT, false); }
      set { this.colors.Set(Constants.COLUMN_HIGHLIGHT, false, value); }
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
