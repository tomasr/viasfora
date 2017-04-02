using System;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora {
  public interface IVsfSettings : IUpdatableSettings {
    bool KeywordClassifierEnabled { get; set; }
    bool FlowControlUseItalics { get; set; }
    bool EscapeSequencesEnabled { get; set; }
    
    bool CurrentColumnHighlightEnabled { get; set; }
    ColumnStyle CurrentColumnHighlightStyle { get; set; }
    double HighlightLineWidth { get; set; }

    bool PresentationModeEnabled { get; set; }
    int PresentationModeDefaultZoom { get; set; }
    int PresentationModeEnabledZoom { get; set; }
    bool PresentationModeIncludeEnvFonts { get; set; }

    bool ModelinesEnabled { get; set; }
    int ModelinesNumLines { get; set; }

    bool DeveloperMarginEnabled { get; set; }
    Outlining.AutoExpandMode AutoExpandRegions { get; set; }
    bool BoldAsItalicsEnabled { get; set; }
    String TextObfuscationRegexes { get; set; }
    bool TelemetryEnabled { get; set; }

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
