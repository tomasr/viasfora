using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CssEscapeSequenceParser : IEscapeSequenceParser {
    private String text;
    private int start;
    public CssEscapeSequenceParser(String text) {
      this.text = text;
      // always skip the first char
      // (since quotes are included in the string)
      this.start = 1;
    }
    public Span? Next() {
      const int maxHexLen = 6;
      while ( start < text.Length - 2 ) {
        if ( text[start] == '\\' ) {
          int len = 1;
          while ( (start+len) < text.Length && IsHexDigit(text[start+len+1]) && len < maxHexLen ) {
            len++;
          }

          Span span = new Span(start, len+1);
          start += len + 1;
          return span;
        }
        start++;
      }
      return null;
    }
    private bool IsHexDigit(char c) {
      if ( Char.IsDigit(c) ) return true;
      return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }
  }
}
