using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Settings {

  [Export(typeof(IVsfSettings))]
  public class VsfSettings : IVsfSettings {
    const String KEYWORD_CLASSIFIER_ENABLED = "KeywordClassifierEnabled";
    const String ESCAPE_SEQ_ENABLED = "EscapeSequencesEnabled";
    const String CURRENT_LINE_ENABLED = "CurrentLineHighlightEnabled";
    const String CURRENT_COLUMN_ENABLED = "CurrentColumnHighlightEnabled";
    const String HIGHLIGHT_LINE_WIDTH = "HighlightLineWidth";
    const String AUTO_EXPAND_REGIONS = "AutoExpandRegions";
    const String BOLD_AS_ITALICS_ENABLED = "BoldAsItalicsEnabled";

    const String TEXT_COMPLETION_ENABLED = "TextCompletionEnabled";
    const String TC_COMPLETE_DURING_TYPING = "TCCompleteDuringTyping";
    const String TC_HANDLE_COMPLETE_WORD = "TCHandleCompleteWord";

    const String XMLNS_PREFIX_ENABLED = "XmlnsPrefixEnabled";
    const String XML_CLOSE_TAG_ENABLED = "XmlCloseTagEnabled";
    const String XML_MATCH_TAGS_ENABLED = "XmlMatchTagsEnabled";

    const String RAINBOW_TAGS_ENABLED = "RainbowTagsEnabled";
    const String RAINBOW_CTRL_TIMER = "RainbowCtrlTimer";
    const String RAINBOW_HIGHLIGHT_MODE = "RainbowHighlightMode";
    const String RAINBOW_TOOLTIPS_ENABLED = "RainbowToolTipsEnabled";

    const String PRESENTATION_MODE_ENABLED = "PresentationModeEnabled";
    const String PRESENTATION_MODE_DEFAULT_ZOOM = "PresentationModeDefaultZoom";
    const String PRESENTATION_MODE_ENABLED_ZOOM = "PresentationModeEnabledZoom";
    const String PRESENTATION_MODE_INCLUDE_ENV_FONTS = "PresentationModeIncludeEnvFonts";
    const String MODELINES_ENABLED = "ModelinesEnabled";
    const String MODELINES_NUMLINES = "ModelinesNumLines";
    const String DEVMARGIN_ENABLED = "DeveloperMarginEnabled";

    const String TEXTOBF_REGEXES = "TextObfuscationRegexes";
    const String TELEMETRY_ENABLED = "TelemetryEnabled";

    private ISettingsStore settings;
    public event EventHandler SettingsChanged;
    
    public bool KeywordClassifierEnabled {
      get { return GetBoolean(KEYWORD_CLASSIFIER_ENABLED, true); }
      set { SetValue(KEYWORD_CLASSIFIER_ENABLED, value); }
    }
    public bool EscapeSeqHighlightEnabled {
      get { return GetBoolean(ESCAPE_SEQ_ENABLED, true); }
      set { SetValue(ESCAPE_SEQ_ENABLED, value); }
    }
    public bool XmlnsPrefixHighlightEnabled {
      get { return GetBoolean(XMLNS_PREFIX_ENABLED, true); }
      set { SetValue(XMLNS_PREFIX_ENABLED, value); }
    }
    public bool XmlCloseTagHighlightEnabled {
      get { return GetBoolean(XML_CLOSE_TAG_ENABLED, true); }
      set { SetValue(XML_CLOSE_TAG_ENABLED, value); }
    }
    public bool XmlMatchTagsEnabled {
      get { return GetBoolean(XML_MATCH_TAGS_ENABLED, true); }
      set { SetValue(XML_MATCH_TAGS_ENABLED, value); }
    }
    public bool CurrentLineHighlightEnabled {
      get { return GetBoolean(CURRENT_LINE_ENABLED, false); }
      set { SetValue(CURRENT_LINE_ENABLED, value); }
    }
    public bool CurrentColumnHighlightEnabled {
      get { return GetBoolean(CURRENT_COLUMN_ENABLED, false); }
      set { SetValue(CURRENT_COLUMN_ENABLED, value); }
    }
    public double HighlightLineWidth {
      get { return GetDouble(HIGHLIGHT_LINE_WIDTH, 1.4); }
      set { SetValue(HIGHLIGHT_LINE_WIDTH, value); }
    }
    public int RainbowDepth {
      get { return GetInt32("RainbowDepth", 4); }
      set { SetValue("RainbowDepth", value); }
    }
    public bool RainbowTagsEnabled {
      get { return GetBoolean(RAINBOW_TAGS_ENABLED, true); }
      set { SetValue(RAINBOW_TAGS_ENABLED, value); }
    }
    public long RainbowCtrlTimer {
      get { return GetInt64(RAINBOW_CTRL_TIMER, 300); }
      set { SetValue(RAINBOW_CTRL_TIMER, value); }
    }
    public RainbowHighlightMode RainbowHighlightMode {
      get { return GetEnum(RAINBOW_HIGHLIGHT_MODE, RainbowHighlightMode.TrackNextScope); }
      set { SetValue(RAINBOW_HIGHLIGHT_MODE, value); }
    }
    public bool RainbowToolTipsEnabled {
      get { return GetBoolean(RAINBOW_TOOLTIPS_ENABLED, true); }
      set { SetValue(RAINBOW_TOOLTIPS_ENABLED, value); }
    }
    public bool PresentationModeEnabled {
      get { return GetBoolean(PRESENTATION_MODE_ENABLED, true); }
      set { SetValue(PRESENTATION_MODE_ENABLED, value); }
    }
    public int PresentationModeDefaultZoomLevel {
      get { return GetInt32(PRESENTATION_MODE_DEFAULT_ZOOM, 100); }
      set { SetValue(PRESENTATION_MODE_DEFAULT_ZOOM, value); }
    }
    public int PresentationModeEnabledZoomLevel {
      get { return GetInt32(PRESENTATION_MODE_ENABLED_ZOOM, 150); }
      set { SetValue(PRESENTATION_MODE_ENABLED_ZOOM, value); }
    }
    public bool PresentationModeIncludeEnvironmentFonts {
      get { return GetBoolean(PRESENTATION_MODE_INCLUDE_ENV_FONTS, false); }
      set { SetValue(PRESENTATION_MODE_INCLUDE_ENV_FONTS, value); }
    }
    public bool ModelinesEnabled {
      get { return GetBoolean(MODELINES_ENABLED, true); }
      set { SetValue(MODELINES_ENABLED, value); }
    }
    public int ModelinesNumLines {
      get { return GetInt32(MODELINES_NUMLINES, 5); }
      set { SetValue(MODELINES_NUMLINES, value); }
    }
    public bool DevMarginEnabled {
      get { return GetBoolean(DEVMARGIN_ENABLED, true); }
      set { SetValue(DEVMARGIN_ENABLED, value); }
    }
    public bool TextCompletionEnabled {
      get { return GetBoolean(TEXT_COMPLETION_ENABLED, true); }
      set { SetValue(TEXT_COMPLETION_ENABLED, value); }
    }
    public bool TCCompleteDuringTyping {
      get { return GetBoolean(TC_COMPLETE_DURING_TYPING, false); }
      set { SetValue(TC_COMPLETE_DURING_TYPING, value); }
    }
    public bool TCHandleCompleteWord {
      get { return GetBoolean(TC_HANDLE_COMPLETE_WORD, false); }
      set { SetValue(TC_HANDLE_COMPLETE_WORD, value); }
    }
    public Outlining.AutoExpandMode AutoExpandRegions {
      get { return GetEnum(AUTO_EXPAND_REGIONS, Outlining.AutoExpandMode.No); }
      set { SetValue(AUTO_EXPAND_REGIONS, value); }
    }
    public bool BoldAsItalicsEnabled {
      get { return GetBoolean(BOLD_AS_ITALICS_ENABLED, false); }
      set { SetValue(BOLD_AS_ITALICS_ENABLED, value); }
    }
    public String TextObfuscationRegexes {
      get { return GetValue(TEXTOBF_REGEXES, ""); }
      set { SetValue(TEXTOBF_REGEXES, value); }
    }
    public bool TelemetryEnabled {
      get { return GetBoolean(TELEMETRY_ENABLED, true); }
      set { SetValue(TELEMETRY_ENABLED, value); }
    }

    [ImportingConstructor]
    public VsfSettings(ISettingsStore store) {
      this.settings = store;
    }
    public void Load() {
      settings.Load();
    }
    public void Save() {
      settings.Save();
      if ( SettingsChanged != null ) {
        SettingsChanged(this, EventArgs.Empty);
      }
    }

    public bool GetBoolean(String name, bool defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToBoolean(val);
    }

    public int GetInt32(String name, int defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : ConvertToInt32(val);
    }
    public long GetInt64(String name, long defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : ConvertToInt64(val);
    }
    public double GetDouble(String name, double defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : ConvertToDouble(val);
    }
    public T GetEnum<T>(String name, T defval) where T : struct {
      String val = settings.Get(name);
      T actual;
      if ( Enum.TryParse<T>(val, out actual) ) {
        return actual;
      }
      return defval;
    }

    public String GetValue(String name, String defValue) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defValue : val;
    }
    public void SetValue(String name, object value) {
      if ( value != null ) {
        settings.Set(name, Convert.ToString(value, CultureInfo.InvariantCulture));
      } else {
        settings.Set(name, null);
      }
    }

    public static double ConvertToDouble(String val) {
      double result;
      var styles = NumberStyles.AllowLeadingWhite
                 | NumberStyles.AllowTrailingWhite
                 | NumberStyles.AllowLeadingSign
                 | NumberStyles.AllowDecimalPoint
                 | NumberStyles.AllowThousands
                 | NumberStyles.AllowExponent;
      if ( !double.TryParse(val, styles, CultureInfo.InvariantCulture, out result) ) {
        return Convert.ToDouble(val, CultureInfo.CurrentCulture);
      }
      return result;
    }
    public static int ConvertToInt32(String val) {
      int result;
      var styles = NumberStyles.Integer;
      if ( !int.TryParse(val, styles, CultureInfo.InvariantCulture, out result) ) {
        return Convert.ToInt32(val, CultureInfo.CurrentCulture);
      }
      return result;
    }
    public static long ConvertToInt64(String val) {
      long result;
      var styles = NumberStyles.Integer;
      if ( !long.TryParse(val, styles, CultureInfo.InvariantCulture, out result) ) {
        return Convert.ToInt64(val, CultureInfo.CurrentCulture);
      }
      return result;
    }
  }
}
