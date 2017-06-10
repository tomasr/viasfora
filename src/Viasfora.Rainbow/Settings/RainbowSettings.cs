using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Rainbow.Settings {
  [Export(typeof(IRainbowSettings))]
  public class RainbowSettings : SettingsBase, IRainbowSettings {
    public int RainbowDepth {
      get { return GetInt32(nameof(RainbowDepth), 4); }
      set { SetValue(nameof(RainbowDepth), value); }
    }
    public bool RainbowTagsEnabled {
      get { return GetBoolean(nameof(RainbowTagsEnabled), true); }
      set { SetValue(nameof(RainbowTagsEnabled), value); }
    }
    public bool RainbowColorize {
      get { return GetBoolean(nameof(RainbowColorize), true); }
      set { SetValue(nameof(RainbowColorize), value); }
    }
    public long RainbowCtrlTimer {
      get { return GetInt64(nameof(RainbowCtrlTimer), 300); }
      set { SetValue(nameof(RainbowCtrlTimer), value); }
    }
    public RainbowHighlightMode RainbowHighlightMode {
      get { return GetEnum(nameof(RainbowHighlightMode), RainbowHighlightMode.TrackNextScope); }
      set { SetValue(nameof(RainbowHighlightMode), value); }
    }
    public bool RainbowToolTipsEnabled {
      get { return GetBoolean(nameof(RainbowToolTipsEnabled), true); }
      set { SetValue(nameof(RainbowToolTipsEnabled), value); }
    }
    public RainbowColoringMode RainbowColoringMode {
      get { return GetEnum(nameof(RainbowColoringMode), RainbowColoringMode.Unified); }
      set { SetValue(nameof(RainbowColoringMode), value); }
    }

    [ImportingConstructor]
    public RainbowSettings(ISettingsStore store, IStorageConversions converter) : base(store, converter) {
    }
  }
}
