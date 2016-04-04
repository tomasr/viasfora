using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.RainbowOptions)]
  public class RainbowOptionsPage : DialogPage {
    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      var rainbowSettings = SettingsContext.GetSpecificSettings<IRainbowSettings>();

      rainbowSettings.RainbowDepth = RainbowDepth;
      rainbowSettings.RainbowTagsEnabled = RainbowTagsEnabled;
      rainbowSettings.RainbowToolTipsEnabled = RainbowToolTipsEnabled;
      rainbowSettings.RainbowHighlightMode = RainbowHighlightMode;
      rainbowSettings.Save();
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      var rainbowSettings = SettingsContext.GetSpecificSettings<IRainbowSettings>();

      RainbowDepth = rainbowSettings.RainbowDepth;
      RainbowTagsEnabled = rainbowSettings.RainbowTagsEnabled;
      RainbowHighlightMode = rainbowSettings.RainbowHighlightMode;
      RainbowToolTipsEnabled = rainbowSettings.RainbowToolTipsEnabled;
    }

    [LocDisplayName("Enable Rainbow Braces")]
    [Description("Highlight matching braces using colors based on nesting")]
    [Category("Rainbow Braces")]
    public bool RainbowTagsEnabled { get; set; }

    private int rainbowDepth;
    [LocDisplayName("Rainbow Depth")]
    [Description("Controls how many different colors are used to render rainbow braces")]
    [Category("Rainbow Braces")]
    public int RainbowDepth {
      get { return this.rainbowDepth; }
      set {
        if ( value <= 0 || value > Constants.MAX_RAINBOW_DEPTH ) {
          throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
        }
        this.rainbowDepth = value;
      }
    }

    [LocDisplayName("Rainbow Highlight Mode")]
    [Description("Controls how the caret position is used to identify braces to highlight.")]
    [Category("Rainbow Braces")]
    public RainbowHighlightMode RainbowHighlightMode { get; set; }

    [LocDisplayName("Enable Rainbow ToolTips")]
    [Description("Show a tooltip highlighting matching braces when you hover the mouse over a rainbow brace")]
    [Category("Rainbow Braces")]
    public bool RainbowToolTipsEnabled { get; set; }

  }
}
