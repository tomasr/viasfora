using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public static class VsColors {
    private static bool assemblyLoadAttempted;
    private static Type envColorsType;
    public static object CommandShelfBackgroundGradientBrushKey { get; private set; }
    public static object CommandBarTextActiveBrushKey { get; private set; }
    public static object CommandBarTextInactiveBrushKey { get; private set; }
    public static object CommandBarTextSelectedBrushKey { get; private set; }
    public static object CommandBarTextHoverBrushKey { get; private set; }
    public static object CommandBarHoverOverSelectedIconBrushKey { get; private set; }
    public static object CommandBarHoverOverSelectedIconBorderBrushKey { get; private set; }
    public static object CommandBarMouseOverBackgroundGradientBrushKey { get; private set; }
    public static object CommandBarMouseOverUnfocusedBrushKey { get; private set; }
    public static object CommandBarSelectedBrushKey { get; private set; }
    public static object CommandBarBorderBrushKey { get; private set; }

    public static object DropDownGlyphBrushKey { get; private set; }
    public static object DropDownMouseOverGlyphBrushKey { get; private set; }
    public static object DropDownMouseDownGlyphBrushKey { get; private set; }

    static VsColors() {
      assemblyLoadAttempted = false;
      envColorsType = null;
      CommandShelfBackgroundGradientBrushKey = Get("CommandShelfBackgroundGradientBrushKey", VsBrushes.CommandBarGradientBeginKey);
      CommandBarTextActiveBrushKey = Get("CommandBarTextActiveBrushKey", VsBrushes.CommandBarTextActiveKey);
      CommandBarTextInactiveBrushKey = Get("CommandBarTextInactiveBrushKey", VsBrushes.CommandBarTextInactiveKey);
      CommandBarTextSelectedBrushKey = Get("CommandBarTextSelectedBrushKey", VsBrushes.CommandBarTextSelectedKey);
      CommandBarTextHoverBrushKey = Get("CommandBarTextHoverBrushKey", VsBrushes.CommandBarTextHoverKey);
      CommandBarHoverOverSelectedIconBrushKey = Get("CommandBarHoverOverSelectedIconBrushKey", VsBrushes.CommandBarHoverOverSelectedIconKey);
      CommandBarHoverOverSelectedIconBorderBrushKey = Get("CommandBarHoverOverSelectedIconBorderBrushKey", VsBrushes.CommandBarHoverOverSelectedIconBorderKey);
      CommandBarMouseOverBackgroundGradientBrushKey  = Get("CommandBarMouseOverBackgroundGradientBrushKey", VsBrushes.CommandBarMouseOverBackgroundGradientKey);
      CommandBarMouseOverUnfocusedBrushKey = Get("CommandBarMouseOverUnfocusedBrushKey", VsBrushes.CommandBarHoverOverSelectedKey);
      CommandBarSelectedBrushKey = Get("CommandBarSelectedBrushKey", VsBrushes.CommandBarSelectedKey);

      CommandBarBorderBrushKey = Get("CommandBarBorderBrushKey", VsBrushes.CommandBarBorderKey);

      DropDownGlyphBrushKey = Get("DropDownGlyphBrushKey", VsBrushes.ComboBoxGlyphKey);
      DropDownMouseOverGlyphBrushKey = Get("DropDownMouseOverGlyphBrushKey", VsBrushes.ComboBoxMouseOverGlyphKey);
      DropDownMouseDownGlyphBrushKey = Get("DropDownMouseDownGlyphBrushKey", VsBrushes.ComboBoxGlyphKey);
    }


    private static object Get(String key, object alternate) {
      if ( !assemblyLoadAttempted ) {
        Assembly vsShellAssembly = null;
        try {
          vsShellAssembly = Assembly.Load("Microsoft.VisualStudio.Shell.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        } catch ( FileNotFoundException ) {
        }
        assemblyLoadAttempted = true;
        if ( vsShellAssembly != null ) {
          envColorsType = vsShellAssembly.GetType("Microsoft.VisualStudio.PlatformUI.EnvironmentColors");
        }
      }
      if ( envColorsType != null ) {
        var prop = envColorsType.GetProperty(key);
        return prop.GetValue(null, null);
      }
      return alternate;
    }
  }
}
