using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CssStringScanner : IStringScanner {
    private String text;
    private int start;
    public CssStringScanner(String text) {
      this.text = text;
      // always skip the first char
      // (since quotes are included in the string)
      this.start = 1;
    }
    public StringPart? Next() {
      const int maxHexLen = 6;
      while ( start < text.Length - 2 ) {
        if ( text[start] == '\\' ) {
          int len = 1;
          while ( (start+len) < text.Length && text[start+len+1].IsHexDigit() && len < maxHexLen ) {
            len++;
          }

          var span = new TextSpan(start, len+1);
          start += len + 1;
          return new StringPart(span, StringPartType.EscapeSequence);
        }
        start++;
      }
      return null;
    }
  }
}
