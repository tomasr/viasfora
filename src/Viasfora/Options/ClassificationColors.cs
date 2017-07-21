using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Drawing;

namespace Winterdom.Viasfora.Options {
  public class ClassificationColors {
    private String classificationName;
    private Color foreground;
    private bool foregroundChanged;
    private Color background;
    private bool backgroundChanged;
    private FontStyles fontFlags;
    private bool fontFlagsChanged;

    public Color Foreground {
      get { return this.Get(true); }
      set { this.Set(value, true); }
    }

    public Color Background {
      get { return this.Get(false); }
      set { this.Set(value, false); }
    }

    public FontStyles Style {
      get { return this.fontFlags; }
      set {
        if ( this.fontFlags != value ) {
          this.fontFlags = value;
          this.fontFlagsChanged = true;
        }
      }
    }

    public ClassificationColors(String name) {
      this.classificationName = name;
    }

    public void Load(ColorStorage colorStorage) {
      ColorableItemInfo[] colors = new ColorableItemInfo[1];
      var hr = colorStorage.Storage.GetItem(classificationName, colors);
      ErrorHandler.ThrowOnFailure(hr);

      this.foregroundChanged = false;
      if ( colors[0].bForegroundValid != 0 ) {
        this.foreground = MapColor(colorStorage, colors[0].crForeground);
      } else {
        this.foreground = Color.Transparent;
      }
      this.backgroundChanged = false;
      if ( colors[0].bBackgroundValid != 0 ) {
        this.background = MapColor(colorStorage, colors[0].crBackground);
      } else {
        this.background = Color.Transparent;
      }
      this.fontFlagsChanged = false;
      if ( colors[0].bFontFlagsValid != 0 ) {
        this.fontFlags = MapFontFlags(colors[0].dwFontFlags);
      }
    }

    public void Save(ColorStorage colorStorage) {
      if ( this.backgroundChanged || this.foregroundChanged ) {
        ColorableItemInfo[] colors = new ColorableItemInfo[1];
        var hr = colorStorage.Storage.GetItem(classificationName, colors);
        ErrorHandler.ThrowOnFailure(hr);

        if ( this.foregroundChanged ) {
          if ( foreground == Color.Transparent ) {
            colors[0].crForeground = colorStorage.GetAutomaticColor();
          } else {
            colors[0].crForeground = (uint)ColorTranslator.ToWin32(foreground);
          }
          colors[0].bForegroundValid = 1;
        }
        if ( this.backgroundChanged ) {
          if ( background == Color.Transparent ) {
            colors[0].crBackground = colorStorage.GetAutomaticColor();
          } else {
            colors[0].crBackground = (uint)ColorTranslator.ToWin32(background);
          }
          colors[0].bBackgroundValid = 1;
        }
        if ( this.fontFlagsChanged ) {
          colors[0].dwFontFlags = ToFontFlags(this.fontFlags);
          colors[0].bFontFlagsValid = 1;
        }

        hr = colorStorage.Storage.SetItem(classificationName, colors);
        ErrorHandler.ThrowOnFailure(hr);
      }
    }

    public Color Get(bool getForeground) {
      return getForeground ? this.foreground : this.background;
    }
    public void Set(Color color, bool isForeground) {
      if ( isForeground && color != this.foreground ) {
        this.foreground = color;
        this.foregroundChanged = true;
      } else if ( color != this.background ) {
        this.background = color;
        this.backgroundChanged = true;
      }
    }

    private Color MapColor(ColorStorage storage, uint colorRef) {
      int type;
      var hr = storage.Utilities.GetColorType(colorRef, out type);
      switch ( (__VSCOLORTYPE)type ) {
        case __VSCOLORTYPE.CT_SYSCOLOR:
        case __VSCOLORTYPE.CT_RAW:
          return FromWin32(colorRef);
        case __VSCOLORTYPE.CT_COLORINDEX:
          return FromColorIndex(storage, colorRef);
        case __VSCOLORTYPE.CT_VSCOLOR:
          return FromVsColor(storage, colorRef);
        case __VSCOLORTYPE.CT_AUTOMATIC:
          return Color.Transparent;
        default:
          throw new InvalidOperationException("Invalid VS color type");
      }
    }

    private Color FromVsColor(ColorStorage storage, uint colorRef) {
      int vsColor;
      var hr = storage.Utilities.GetEncodedVSColor(colorRef, out vsColor);
      ErrorHandler.ThrowOnFailure(hr);

      uint rgb;
      hr = storage.Shell.GetVSSysColorEx(vsColor, out rgb);
      ErrorHandler.ThrowOnFailure(hr);

      return FromWin32(rgb);
    }

    private Color FromColorIndex(ColorStorage storage, uint colorRef) {
      COLORINDEX[] index = new COLORINDEX[1];
      var hr = storage.Utilities.GetEncodedIndex(colorRef, index);
      ErrorHandler.ThrowOnFailure(hr);

      // useful when the color is marked as "automatic" in VS
      if ( index[0] == COLORINDEX.CI_SYSTEXT_BK || index[0] == COLORINDEX.CI_SYSTEXT_FG )
        return Color.Transparent;

      uint rgb;
      hr = storage.Utilities.GetRGBOfIndex(index[0], out rgb);
      ErrorHandler.ThrowOnFailure(hr);
      return FromWin32(rgb);
    }

    private Color FromWin32(uint colorRef) {
      return ColorTranslator.FromWin32((int)colorRef);
    }

    private FontStyles MapFontFlags(uint dwFontFlags) {
      var style = FontStyles.None;
      if ( (dwFontFlags & (uint)FONTFLAGS.FF_BOLD) != 0 )
        style |= FontStyles.Bold;
      if ( (dwFontFlags & (uint)FONTFLAGS.FF_STRIKETHROUGH) != 0 )
        style |= FontStyles.Strikethrough;
      return style;
    }

    private uint ToFontFlags(FontStyles style) {
      uint dwFontFlags = 0;
      if ( style.HasFlag(FontStyles.Bold) )
        dwFontFlags |= (uint)FONTFLAGS.FF_BOLD;
      if ( style.HasFlag(FontStyles.Strikethrough) )
        dwFontFlags |= (uint)FONTFLAGS.FF_STRIKETHROUGH;
      return dwFontFlags;
    }
  }


}
