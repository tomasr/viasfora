using System;
using System.ComponentModel.Composition;
using System.Globalization;

namespace Winterdom.Viasfora.Settings {

  [Export(typeof(IVsfSettings))]
  public class VsfSettings : IVsfSettings {

    private ISettingsStore settings;
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
    public bool TextCompletionEnabled {
      get { return GetBoolean(nameof(TextCompletionEnabled), true); }
      set { SetValue(nameof(TextCompletionEnabled), value); }
    }
    public bool TCCompleteDuringTyping {
      get { return GetBoolean(nameof(TCCompleteDuringTyping), false); }
      set { SetValue(nameof(TCCompleteDuringTyping), value); }
    }
    public bool TCHandleCompleteWord {
      get { return GetBoolean(nameof(TCHandleCompleteWord), false); }
      set { SetValue(nameof(TCHandleCompleteWord), value); }
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
