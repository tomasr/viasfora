using System;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora {
  public interface IVsfSettings : IUpdatableSettings {
    bool KeywordClassifierEnabled { get; set; }
    bool FlowControlKeywordsEnabled { get; set; }
    bool VisibilityKeywordsEnabled { get; set; }
    bool QueryKeywordsEnabled { get; set; }
    bool FlowControlUseItalics { get; set; }
    bool EscapeSequencesEnabled { get; set; }

    bool ArgumentValidationClassifierEnabled { get; set; }
    bool MethodOverloadsClassifierEnabled { get; set; }

    bool CurrentLineHighlightEnabled { get; set; }
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

    void Load();
    void Save();
  }
}
