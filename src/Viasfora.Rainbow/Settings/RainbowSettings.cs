using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Rainbow.Settings {
  [Export(typeof(IRainbowSettings))]
  public class RainbowSettings : IRainbowSettings {

    private IVsfSettings settings;

    public event EventHandler SettingsChanged {
      add { settings.SettingsChanged += value; }
      remove { settings.SettingsChanged -= value; }
    }

    public int RainbowDepth {
      get { return settings.GetInt32(nameof(RainbowDepth), 4); }
      set { settings.SetValue(nameof(RainbowDepth), value); }
    }
    public bool RainbowTagsEnabled {
      get { return settings.GetBoolean(nameof(RainbowTagsEnabled), true); }
      set { settings.SetValue(nameof(RainbowTagsEnabled), value); }
    }
    public long RainbowCtrlTimer {
      get { return settings.GetInt64(nameof(RainbowCtrlTimer), 300); }
      set { settings.SetValue(nameof(RainbowCtrlTimer), value); }
    }
    public RainbowHighlightMode RainbowHighlightMode {
      get { return settings.GetEnum(nameof(RainbowHighlightMode), RainbowHighlightMode.TrackNextScope); }
      set { settings.SetValue(nameof(RainbowHighlightMode), value); }
    }
    public bool RainbowToolTipsEnabled {
      get { return settings.GetBoolean(nameof(RainbowToolTipsEnabled), true); }
      set { settings.SetValue(nameof(RainbowToolTipsEnabled), value); }
    }

    [ImportingConstructor]
    public RainbowSettings(IVsfSettings settings) {
      this.settings = settings;
    }
  }
}
