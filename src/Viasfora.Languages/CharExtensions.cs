using System;

namespace Winterdom.Viasfora.Languages {
  public static class CharExtensions {
    public static bool IsHexDigit(this char c) {
      if ( Char.IsDigit(c) ) return true;
      return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }
    public static bool IsEndOfLine(this char c) {
      return c == '\r' || c == '\n';
    }
  }
}
