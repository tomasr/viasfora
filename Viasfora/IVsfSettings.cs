using System;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora {
  public interface IVsfSettings {
    bool KeywordClassifierEnabled { get; set; }
    bool EscapeSeqHighlightEnabled { get; set; }
    
    bool XmlnsPrefixHighlightEnabled { get; set; }
    bool XmlCloseTagHighlightEnabled { get; set; }
    bool XmlMatchTagsEnabled { get; set; }

    bool CurrentLineHighlightEnabled { get; set; }
    bool CurrentColumnHighlightEnabled { get; set; }
    double HighlightLineWidth { get; set; }

    int RainbowDepth { get; set; }
    bool RainbowTagsEnabled { get; set; }
    long RainbowCtrlTimer { get; set; }
    RainbowHighlightMode RainbowHighlightMode { get; set; }
    bool RainbowToolTipsEnabled { get; set; }

    bool PresentationModeEnabled { get; set; }
    int PresentationModeDefaultZoomLevel { get; set; }
    int PresentationModeEnabledZoomLevel { get; set; }
    bool PresentationModeIncludeEnvironmentFonts { get; set; }

    bool ModelinesEnabled { get; set; }
    int ModelinesNumLines { get; set; }

    bool DevMarginEnabled { get; set; }
    bool TextCompletionEnabled { get; set; }
    bool TCCompleteDuringTyping { get; set; }
    bool TCHandleCompleteWord { get; set; }
    Outlining.AutoExpandMode AutoExpandRegions { get; set; }
    bool BoldAsItalicsEnabled { get; set; }
    String TextObfuscationRegexes { get; set; }
    bool TelemetryEnabled { get; set; }

    event EventHandler SettingsChanged;
    String GetValue(String name, String defaultValue);
    bool GetBoolean(String name, bool defval);
    int GetInt32(String name, int defval);
    long GetInt64(String name, long defval);
    double GetDouble(String name, double defval);
    T GetEnum<T>(String name, T defval) where T : struct;
    void SetValue(String name, object value);
    void Load();
    void Save();
  }
}
