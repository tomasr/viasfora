using System;
using System.ComponentModel.Composition;
using System.Globalization;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Settings {

  [Export(typeof(IVsfSettings))]
  public class VsfSettings : SettingsBase, IVsfSettings {
    public event EventHandler SettingsChanged;
    
    public bool KeywordClassifierEnabled {
      get { return GetBoolean(nameof(KeywordClassifierEnabled), true); }
      set { SetValue(nameof(KeywordClassifierEnabled), value); }
    }
    public bool FlowControlUseItalics {
      get { return GetBoolean(nameof(FlowControlUseItalics), false); }
      set { SetValue(nameof(FlowControlUseItalics), value); }
    }
    public bool EscapeSequencesEnabled {
      get { return GetBoolean(nameof(EscapeSequencesEnabled), true); }
      set { SetValue(nameof(EscapeSequencesEnabled), value); }
    }
    public bool CurrentColumnHighlightEnabled {
      get { return GetBoolean(nameof(CurrentColumnHighlightEnabled), false); }
      set { SetValue(nameof(CurrentColumnHighlightEnabled), value); }
    }
    public ColumnStyle CurrentColumnHighlightStyle {
      get { return GetEnum<ColumnStyle>(nameof(CurrentColumnHighlightStyle), ColumnStyle.FullBorder); }
      set { SetValue(nameof(CurrentColumnHighlightStyle), value); }
    }
    public double HighlightLineWidth {
      get { return GetDouble(nameof(HighlightLineWidth), 1.4); }
      set { SetValue(nameof(HighlightLineWidth), value); }
    }
    public bool PresentationModeEnabled {
      get { return GetBoolean(nameof(PresentationModeEnabled), true); }
      set { SetValue(nameof(PresentationModeEnabled), value); }
    }
    public int PresentationModeDefaultZoom {
      get { return GetInt32(nameof(PresentationModeDefaultZoom), 100); }
      set { SetValue(nameof(PresentationModeDefaultZoom), value); }
    }
    public int PresentationModeEnabledZoom {
      get { return GetInt32(nameof(PresentationModeEnabledZoom), 150); }
      set { SetValue(nameof(PresentationModeEnabledZoom), value); }
    }
    public bool PresentationModeIncludeEnvFonts {
      get { return GetBoolean(nameof(PresentationModeIncludeEnvFonts), false); }
      set { SetValue(nameof(PresentationModeIncludeEnvFonts), value); }
    }
    public bool ModelinesEnabled {
      get { return GetBoolean(nameof(ModelinesEnabled), true); }
      set { SetValue(nameof(ModelinesEnabled), value); }
    }
    public int ModelinesNumLines {
      get { return GetInt32(nameof(ModelinesNumLines), 5); }
      set { SetValue(nameof(ModelinesNumLines), value); }
    }
    public bool DeveloperMarginEnabled {
      get { return GetBoolean(nameof(DeveloperMarginEnabled), true); }
      set { SetValue(nameof(DeveloperMarginEnabled), value); }
    }
    public Outlining.AutoExpandMode AutoExpandRegions {
      get { return GetEnum(nameof(AutoExpandRegions), Outlining.AutoExpandMode.No); }
      set { SetValue(nameof(AutoExpandRegions), value); }
    }
    public bool BoldAsItalicsEnabled {
      get { return GetBoolean(nameof(BoldAsItalicsEnabled), false); }
      set { SetValue(nameof(BoldAsItalicsEnabled), value); }
    }
    public String TextObfuscationRegexes {
      get { return GetValue(nameof(TextObfuscationRegexes), ""); }
      set { SetValue(nameof(TextObfuscationRegexes), value); }
    }
    public bool TelemetryEnabled {
      get { return GetBoolean(nameof(TelemetryEnabled), true); }
      set { SetValue(nameof(TelemetryEnabled), value); }
    }

    [ImportingConstructor]
    public VsfSettings(ISettingsStore store, IStorageConversions converter)
      : base(store, converter) {
    }
    public void Load() {
      Store.Load();
    }
    public void Save() {
      Store.Save();
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

  }
}
