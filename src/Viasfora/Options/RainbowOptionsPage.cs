using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.RainbowOptions)]
  public class RainbowOptionsPage : DialogPage {
    private ClassificationList colors;

    public override void SaveSettingsToStorage() {
      var rainbowSettings = SettingsContext.GetService<IRainbowSettings>();

      rainbowSettings.RainbowDepth = RainbowDepth;
      rainbowSettings.RainbowTagsEnabled = RainbowTagsEnabled;
      rainbowSettings.RainbowColorize = RainbowColorize;
      rainbowSettings.RainbowToolTipsEnabled = RainbowToolTipsEnabled;
      rainbowSettings.RainbowHighlightMode = RainbowHighlightMode;
      rainbowSettings.RainbowColoringMode = RainbowColoringMode;
      rainbowSettings.RainbowHighlightKey = RainbowHighlightKey;
      rainbowSettings.Save();

      colors.Save();
    }
    public override void LoadSettingsFromStorage() {
      var rainbowSettings = SettingsContext.GetService<IRainbowSettings>();

      RainbowDepth = rainbowSettings.RainbowDepth;
      RainbowTagsEnabled = rainbowSettings.RainbowTagsEnabled;
      RainbowColorize = rainbowSettings.RainbowColorize;
      RainbowHighlightMode = rainbowSettings.RainbowHighlightMode;
      RainbowToolTipsEnabled = rainbowSettings.RainbowToolTipsEnabled;
      RainbowColoringMode = rainbowSettings.RainbowColoringMode;
      RainbowHighlightKey = rainbowSettings.RainbowHighlightKey;

      this.colors = new ClassificationList(new ColorStorage(this.Site));
      colors.Load(
        Rainbows.Rainbow1,
        Rainbows.Rainbow2,
        Rainbows.Rainbow3,
        Rainbows.Rainbow4,
        Rainbows.Rainbow5,
        Rainbows.Rainbow6,
        Rainbows.Rainbow7,
        Rainbows.Rainbow8,
        Rainbows.Rainbow9,
        Rainbows.RainbowError,
        Rainbows.TipHilight
        );
    }

    [LocDisplayName("Enable Rainbow Braces")]
    [Description("Enables/Disables all rainbow braces features")]
    [Category("Rainbow Braces")]
    public bool RainbowTagsEnabled { get; set; }

    [LocDisplayName("Colorize Rainbow Braces")]
    [Description("Highlight matching braces using colors based on nesting")]
    [Category("Rainbow Braces")]
    public bool RainbowColorize { get; set; }

    private int rainbowDepth;
    [LocDisplayName("Rainbow Depth")]
    [Description("Controls how many different colors are used to render rainbow braces")]
    [Category("Rainbow Braces")]
    public int RainbowDepth {
      get { return this.rainbowDepth; }
      set {
        if ( value <= 0 || value > Rainbows.MaxDepth ) {
          throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
        }
        this.rainbowDepth = value;
      }
    }

    [LocDisplayName("Rainbow Highlight Mode")]
    [Description("Controls how the caret position is used to identify braces to highlight.")]
    [Category("Rainbow Braces")]
    public RainbowHighlightMode RainbowHighlightMode { get; set; }

    [LocDisplayName("Rainbow Highlight Key")]
    [Description("Controls what key triggers Rainbow Hilights.")]
    [Category("Rainbow Braces")]
    public RainbowHighlightKey RainbowHighlightKey { get; set; }

    [LocDisplayName("Enable Rainbow ToolTips")]
    [Description("Show a tooltip highlighting matching braces when you hover the mouse over a rainbow brace")]
    [Category("Rainbow Braces")]
    public bool RainbowToolTipsEnabled { get; set; }

    [LocDisplayName("Rainbow Coloring Mode")]
    [Description("Controls if brace coloring is based on global or per-brace depth")] 
    [Category("Rainbow Braces")]
    public RainbowColoringMode RainbowColoringMode { get; set; }


    [LocDisplayName("Level 1")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level1 {
      get { return colors.Get(Rainbows.Rainbow1, true); }
      set { colors.Set(Rainbows.Rainbow1, true, value); }
    }
    [LocDisplayName("Level 2")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level2 {
      get { return colors.Get(Rainbows.Rainbow2, true); }
      set { colors.Set(Rainbows.Rainbow2, true, value); }
    }
    [LocDisplayName("Level 3")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level3 {
      get { return colors.Get(Rainbows.Rainbow3, true); }
      set { colors.Set(Rainbows.Rainbow3, true, value); }
    }
    [LocDisplayName("Level 4")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level4 {
      get { return colors.Get(Rainbows.Rainbow4, true); }
      set { colors.Set(Rainbows.Rainbow4, true, value); }
    }
    [LocDisplayName("Level 5")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level5 {
      get { return colors.Get(Rainbows.Rainbow5, true); }
      set { colors.Set(Rainbows.Rainbow5, true, value); }
    }
    [LocDisplayName("Level 6")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level6 {
      get { return colors.Get(Rainbows.Rainbow6, true); }
      set { colors.Set(Rainbows.Rainbow6, true, value); }
    }
    [LocDisplayName("Level 7")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level7 {
      get { return colors.Get(Rainbows.Rainbow7, true); }
      set { colors.Set(Rainbows.Rainbow7, true, value); }
    }
    [LocDisplayName("Level 8")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level8 {
      get { return colors.Get(Rainbows.Rainbow8, true); }
      set { colors.Set(Rainbows.Rainbow8, true, value); }
    }
    [LocDisplayName("Level 9")]
    [Description("Colors to use to highlight braces at this level")]
    [Category("Rainbow Colors")]
    public Color Level9 {
      get { return colors.Get(Rainbows.Rainbow9, true); }
      set { colors.Set(Rainbows.Rainbow9, true, value); }
    }
    [Description("Colors to use to highlight brace errors")]
    [Category("Rainbow Colors")]
    public Color Errors {
      get { return colors.Get(Rainbows.RainbowError, false); }
      set { colors.Set(Rainbows.RainbowError, false, value); }
    }
    [LocDisplayName("Tip Highlight")]
    [Description("Colors to use to highlight rainbow tips")]
    [Category("Rainbow Colors")]
    public Color TipHighlight {
      get { return colors.Get(Rainbows.TipHilight, false); }
      set { colors.Set(Rainbows.TipHilight, false, value); }
    }
  }
}
