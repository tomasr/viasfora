using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Rainbow.Settings {
  [Export(typeof(IRainbowSettings))]
  public class RainbowSettings : SettingsBase, IRainbowSettings {
    public int RainbowDepth {
      get { return this.Store.GetInt32(nameof(RainbowDepth), 4); }
      set { this.Store.SetValue(nameof(RainbowDepth), value); }
    }
    public bool RainbowTagsEnabled {
      get { return this.Store.GetBoolean(nameof(RainbowTagsEnabled), true); }
      set { this.Store.SetValue(nameof(RainbowTagsEnabled), value); }
    }
    public bool RainbowColorize {
      get { return this.Store.GetBoolean(nameof(RainbowColorize), true); }
      set { this.Store.SetValue(nameof(RainbowColorize), value); }
    }
    public long RainbowCtrlTimer {
      get { return this.Store.GetInt64(nameof(RainbowCtrlTimer), 300); }
      set { this.Store.SetValue(nameof(RainbowCtrlTimer), value); }
    }
    public RainbowHighlightMode RainbowHighlightMode {
      get { return this.Store.GetEnum(nameof(RainbowHighlightMode), RainbowHighlightMode.TrackNextScope); }
      set { this.Store.SetValue(nameof(RainbowHighlightMode), value); }
    }
    public RainbowHighlightKey RainbowHighlightKey {
      get { return this.Store.GetEnum(nameof(RainbowHighlightKey), RainbowHighlightKey.LeftCtrl); }
      set { this.Store.SetValue(nameof(RainbowHighlightKey), value); }
    }
    public bool RainbowToolTipsEnabled {
      get { return this.Store.GetBoolean(nameof(RainbowToolTipsEnabled), true); }
      set { this.Store.SetValue(nameof(RainbowToolTipsEnabled), value); }
    }
    public RainbowColoringMode RainbowColoringMode {
      get { return this.Store.GetEnum(nameof(RainbowColoringMode), RainbowColoringMode.Unified); }
      set { this.Store.SetValue(nameof(RainbowColoringMode), value); }
    }
    public bool RainbowLinesEnabled {
      get { return this.Store.GetBoolean(nameof(RainbowLinesEnabled), true); }
      set { this.Store.SetValue(nameof(RainbowLinesEnabled), value); }
    }

    [ImportingConstructor]
    public RainbowSettings(ITypedSettingsStore store) : base(store) {
    }
  }
}
