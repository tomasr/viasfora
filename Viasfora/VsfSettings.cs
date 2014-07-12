using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora {

  public static class VsfSettings {
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

    const String PRESENTATION_MODE_ENABLED = "PresentationModeEnabled";
    const String PRESENTATION_MODE_DEFAULT_ZOOM = "PresentationModeDefaultZoom";
    const String PRESENTATION_MODE_ENABLED_ZOOM = "PresentationModeEnabledZoom";
    const String MODELINES_ENABLED = "ModelinesEnabled";
    const String MODELINES_NUMLINES = "ModelinesNumLines";
    const String DEVMARGIN_ENABLED = "DeveloperMarginEnabled";

    const String TEXTOBF_REGEXES = "TextObfuscationRegexes";

    private static ISettingsStore settings = VsfPackage.GetGlobalSettingsStore();
    public static event EventHandler SettingsUpdated;
    
    public static bool KeywordClassifierEnabled {
      get { return GetBoolean(KEYWORD_CLASSIFIER_ENABLED, true); }
      set { SetValue(KEYWORD_CLASSIFIER_ENABLED, value); }
    }
    public static bool EscapeSeqHighlightEnabled {
      get { return GetBoolean(ESCAPE_SEQ_ENABLED, true); }
      set { SetValue(ESCAPE_SEQ_ENABLED, value); }
    }
    public static bool XmlnsPrefixHighlightEnabled {
      get { return GetBoolean(XMLNS_PREFIX_ENABLED, true); }
      set { SetValue(XMLNS_PREFIX_ENABLED, value); }
    }
    public static bool XmlCloseTagHighlightEnabled {
      get { return GetBoolean(XML_CLOSE_TAG_ENABLED, true); }
      set { SetValue(XML_CLOSE_TAG_ENABLED, value); }
    }
    public static bool XmlMatchTagsEnabled {
      get { return GetBoolean(XML_MATCH_TAGS_ENABLED, true); }
      set { SetValue(XML_MATCH_TAGS_ENABLED, value); }
    }
    public static bool CurrentLineHighlightEnabled {
      get { return GetBoolean(CURRENT_LINE_ENABLED, false); }
      set { SetValue(CURRENT_LINE_ENABLED, value); }
    }
    public static bool CurrentColumnHighlightEnabled {
      get { return GetBoolean(CURRENT_COLUMN_ENABLED, false); }
      set { SetValue(CURRENT_COLUMN_ENABLED, value); }
    }
    public static double HighlightLineWidth {
      get { return GetDouble(HIGHLIGHT_LINE_WIDTH, 1.4); }
      set { SetValue(HIGHLIGHT_LINE_WIDTH, value); }
    }
    public static bool RainbowTagsEnabled {
      get { return GetBoolean(RAINBOW_TAGS_ENABLED, true); }
      set { SetValue(RAINBOW_TAGS_ENABLED, value); }
    }
    public static long RainbowCtrlTimer {
      get { return GetInt64(RAINBOW_CTRL_TIMER, 300); }
      set { SetValue(RAINBOW_CTRL_TIMER, value); }
    }
    public static RainbowHighlightMode RainbowHighlightMode {
      get { return GetEnum(RAINBOW_HIGHLIGHT_MODE, RainbowHighlightMode.TrackNextScope); }
      set { SetValue(RAINBOW_HIGHLIGHT_MODE, value); }
    }
    public static bool PresentationModeEnabled {
      get { return GetBoolean(PRESENTATION_MODE_ENABLED, true); }
      set { SetValue(PRESENTATION_MODE_ENABLED, value); }
    }
    public static int PresentationModeDefaultZoomLevel {
      get { return GetInt32(PRESENTATION_MODE_DEFAULT_ZOOM, 100); }
      set { SetValue(PRESENTATION_MODE_DEFAULT_ZOOM, value); }
    }
    public static int PresentationModeEnabledZoomLevel {
      get { return GetInt32(PRESENTATION_MODE_ENABLED_ZOOM, 150); }
      set { SetValue(PRESENTATION_MODE_ENABLED_ZOOM, value); }
    }
    public static bool ModelinesEnabled {
      get { return GetBoolean(MODELINES_ENABLED, true); }
      set { SetValue(MODELINES_ENABLED, value); }
    }
    public static int ModelinesNumLines {
      get { return GetInt32(MODELINES_NUMLINES, 5); }
      set { SetValue(MODELINES_NUMLINES, value); }
    }
    public static bool DevMarginEnabled {
      get { return GetBoolean(DEVMARGIN_ENABLED, true); }
      set { SetValue(DEVMARGIN_ENABLED, value); }
    }
    public static bool TextCompletionEnabled {
      get { return GetBoolean(TEXT_COMPLETION_ENABLED, true); }
      set { SetValue(TEXT_COMPLETION_ENABLED, value); }
    }
    public static bool TCCompleteDuringTyping {
      get { return GetBoolean(TC_COMPLETE_DURING_TYPING, true); }
      set { SetValue(TC_COMPLETE_DURING_TYPING, value); }
    }
    public static bool TCHandleCompleteWord {
      get { return GetBoolean(TC_HANDLE_COMPLETE_WORD, false); }
      set { SetValue(TC_HANDLE_COMPLETE_WORD, value); }
    }
    public static Text.AutoExpandMode AutoExpandRegions {
      get { return GetEnum(AUTO_EXPAND_REGIONS, Text.AutoExpandMode.No); }
      set { SetValue(AUTO_EXPAND_REGIONS, value); }
    }
    public static bool BoldAsItalicsEnabled {
      get { return GetBoolean(BOLD_AS_ITALICS_ENABLED, false); }
      set { SetValue(BOLD_AS_ITALICS_ENABLED, value); }
    }
    public static String TextObfuscationRegexes {
      get { return GetValue(TEXTOBF_REGEXES, ""); }
      set { SetValue(TEXTOBF_REGEXES, value); }
    }

    public static void Save() {
      settings.Save();
      if ( SettingsUpdated != null ) {
        SettingsUpdated(VsfPackage.Instance, EventArgs.Empty);
      }
    }

    private static bool GetBoolean(String name, bool defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToBoolean(val);
    }

    private static int GetInt32(String name, int defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToInt32(val);
    }
    private static long GetInt64(String name, long defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToInt64(val);
    }
    private static double GetDouble(String name, double defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToDouble(val);
    }
    private static T GetEnum<T>(String name, T defval) where T : struct {
      String val = settings.Get(name);
      T actual;
      if ( Enum.TryParse<T>(val, out actual) ) {
        return actual;
      }
      return defval;
    }

    public static String GetValue(String name, String defValue) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defValue : val;
    }
    public static void SetValue(String name, object value) {
      if ( value != null ) {
        settings.Set(name, Convert.ToString(value));
      } else {
        settings.Set(name, null);
      }
    }
  }
}
