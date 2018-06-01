using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Util {
  // See http://msdn.microsoft.com/en-us/library/vstudio/microsoft.visualstudio.platformui.environmentcolors.aspx
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
    public static object CommandBarMouseDownBackgroundGradientBrushKey { get; private set; }
    public static object CommandBarMouseOverUnfocusedBrushKey { get; private set; }
    public static object CommandBarSelectedBrushKey { get; private set; }
    public static object CommandBarSelectedBorderBrushKey { get; private set; }
    public static object CommandBarBorderBrushKey { get; private set; }

    public static object DropDownGlyphBrushKey { get; private set; }
    public static object DropDownMouseOverGlyphBrushKey { get; private set; }
    public static object DropDownMouseDownGlyphBrushKey { get; private set; }
    public static object DropDownMouseOverBorderBrushKey { get; private set; }
    public static object DropDownMouseOverBackgroundBeginBrushKey { get; private set; }
    public static object DropDownMouseDownBorderBrushKey { get; private set; }
    public static object DropDownMouseDownBackgroundBrushKey { get; private set; }
    public static object DropDownBackgroundBrushKey { get; private set; }
    public static object DropDownBorderBrushKey { get; private set; }
    public static object DropDownDisabledBackgroundBrushKey { get; private set; }
    public static object DropDownDisabledBorderBrushKey { get; private set; }

    public static object DropDownButtonMouseDownBackgroundBrushKey { get; private set; }
    public static object DropDownButtonMouseOverBackgroundBrushKey { get; private set; }
    public static object DropDownButtonMouseDownSeparatorBrushKey { get; private set; }
    public static object DropDownButtonMouseOverSeparatorBrushKey { get; private set; }
    public static object DropDownPopupBackgroundBeginBrushKey { get; private set; }
    public static object DropDownPopupBorderBrushKey { get; private set; }

    public static object ComboBoxBackgroundBrushKey { get; set; }
    public static object ComboBoxMouseOverBackgroundBeginBrushKey { get; set; }
    public static object ComboBoxMouseOverBorderBrushKey { get; set; }

    public static object ToolTipBrushKey { get; set; }
    public static object ToolTipTextBrushKey { get; set; }
    public static object PanelHyperlinkBrushKey { get; set; }

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
      CommandBarMouseDownBackgroundGradientBrushKey  = Get("CommandBarMouseDownBackgroundGradientBrushKey", VsBrushes.CommandBarMouseDownBackgroundGradientKey);
      CommandBarMouseOverUnfocusedBrushKey = Get("CommandBarMouseOverUnfocusedBrushKey", VsBrushes.CommandBarHoverOverSelectedKey);
      CommandBarSelectedBrushKey = Get("CommandBarSelectedBrushKey", VsBrushes.CommandBarSelectedKey);
      CommandBarSelectedBorderBrushKey = Get("CommandBarSelectedBorderBrushKey", VsBrushes.CommandBarSelectedBorderKey);

      CommandBarBorderBrushKey = Get("CommandBarBorderBrushKey", VsBrushes.CommandBarBorderKey);

      DropDownGlyphBrushKey = Get("DropDownGlyphBrushKey", VsBrushes.ComboBoxGlyphKey);
      DropDownMouseOverGlyphBrushKey = Get("DropDownMouseOverGlyphBrushKey", VsBrushes.ComboBoxMouseOverGlyphKey);
      DropDownMouseDownGlyphBrushKey = Get("DropDownMouseDownGlyphBrushKey", VsBrushes.ComboBoxGlyphKey);
      DropDownMouseOverBorderBrushKey = Get("DropDownMouseOverBorderBrushKey", VsBrushes.DropDownMouseOverBorderKey);
      DropDownMouseOverBackgroundBeginBrushKey = Get("DropDownMouseOverBackgroundBeginBrushKey", VsBrushes.DropDownBackgroundKey);
      DropDownMouseDownBorderBrushKey = Get("DropDownMouseDownBorderBrushKey", VsBrushes.DropDownMouseDownBorderKey);
      DropDownMouseDownBackgroundBrushKey = Get("DropDownMouseDownBackgroundBrushKey", VsBrushes.DropDownMouseDownBackgroundKey);
      DropDownBackgroundBrushKey = Get("DropDownBackgroundBrushKey", VsBrushes.DropDownBackgroundKey);
      DropDownBorderBrushKey = Get("DropDownBorderBrushKey", VsBrushes.DropDownBorderKey);
      DropDownDisabledBackgroundBrushKey = Get("DropDownDisabledBackgroundBrushKey", VsBrushes.DropDownDisabledBackgroundKey);
      DropDownDisabledBorderBrushKey = Get("DropDownDisabledBorderBrushKey", VsBrushes.DropDownDisabledBorderKey);

      DropDownButtonMouseDownBackgroundBrushKey = Get("DropDownButtonMouseDownBackgroundBrushKey", VsBrushes.DropDownMouseDownBackgroundKey);
      DropDownButtonMouseOverBackgroundBrushKey = Get("DropDownButtonMouseOverBackgroundBrushKey", VsBrushes.DropDownMouseOverBackgroundGradientKey);
      DropDownButtonMouseDownSeparatorBrushKey = Get("DropDownButtonMouseDownSeparatorBrushKey", VsBrushes.DropDownMouseDownBackgroundKey);
      DropDownButtonMouseOverSeparatorBrushKey = Get("DropDownButtonMouseOverSeparatorBrushKey", VsBrushes.DropDownMouseOverBackgroundGradientKey);

      DropDownPopupBackgroundBeginBrushKey = Get("DropDownPopupBackgroundBeginBrushKey", VsBrushes.DropDownPopupBackgroundGradientKey);
      DropDownPopupBorderBrushKey = Get("DropDownPopupBorderBrushKey", VsBrushes.DropDownPopupBorderKey);

      ComboBoxMouseOverBackgroundBeginBrushKey = Get("ComboBoxMouseOverBackgroundBeginBrushKey", VsBrushes.ComboBoxMouseOverBackgroundBeginKey);
      ComboBoxBackgroundBrushKey = Get("ComboBoxBackgroundBrushKey", VsBrushes.ComboBoxBackgroundKey);
      ComboBoxMouseOverBorderBrushKey = Get("ComboBoxMouseOverBorderBrushKey", VsBrushes.ComboBoxBorderKey);

      // these don't really seem to have a match in VS2010...
      // so just watch it!
      ToolTipBrushKey = Get("ToolTipBorderBrushKey", SystemColors.ControlLightBrushKey);
      ToolTipTextBrushKey = Get("ToolTipTextBrushKey", SystemColors.InfoTextBrushKey);
      PanelHyperlinkBrushKey = Get("PanelHyperlinkBrushKey", SystemColors.HotTrackBrushKey);
    }


    private static object Get(String key, object alternate) {
      if ( !assemblyLoadAttempted ) {
        LoadAssemblyAndType();
        assemblyLoadAttempted = true;
      }
      if ( envColorsType != null ) {
        var prop = envColorsType.GetProperty(key);
        return prop.GetValue(null, null);
      }
      return alternate;
    }

    private static void LoadAssemblyAndType() {
      Assembly vsShellAssembly = null;
      try {
        vsShellAssembly = Assembly.Load("Microsoft.VisualStudio.Shell.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
      } catch ( FileNotFoundException ) {
      }
      if ( vsShellAssembly != null ) {
        envColorsType = vsShellAssembly.GetType("Microsoft.VisualStudio.PlatformUI.EnvironmentColors");
      }
    }
  }
}
