using System;

namespace Winterdom.Viasfora.Rainbow {
  public interface IRainbowSettings : IUpdatableSettings {
    int RainbowDepth { get; set; }
    bool RainbowTagsEnabled { get; set; }
    bool RainbowColorize { get; set; }
    long RainbowCtrlTimer { get; set; }
    RainbowHighlightMode RainbowHighlightMode { get; set; }
    RainbowHighlightKey RainbowHighlightKey { get; set; }
    bool RainbowToolTipsEnabled { get; set; }
    RainbowColoringMode RainbowColoringMode { get; set; }
    bool RainbowLinesEnabled { get; set; }
    RainbowLinesMode RainbowLinesMode { get; set; }
    void Save();
  }

  public enum RainbowLinesMode {
    All = 0,
    MultiLineOnly = 1
  }
}
