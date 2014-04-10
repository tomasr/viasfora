using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CEscapeSequenceParser : IEscapeSequenceParser {
    private String text;
    private int start;
    public CEscapeSequenceParser(String text) {
      this.text = text;
      // always skip the first char
      // (since quotes are included in the string)
      this.start = 1;
      // If this is an at-string, or a C preprocessor include
      // skip it
      if ( text.StartsWith("@") || text.StartsWith("<") )
        this.start = text.Length;
    }
    public Span? Next() {
      while ( start < text.Length - 2 ) {
        if ( text[start] == '\\' ) {
          int len = 1;
          int maxlen = Int32.MaxValue;
          char f = text[start + 1];
          // not perfect, but close enough for first version
          if ( f == 'x' || f == 'X' || f == 'u' || f == 'U' ) {
            while ( (start + len) < text.Length && IsHexDigit(text[start + len + 1]) ) {
              len++;
            }
          }
          if ( f == 'u' ) maxlen = 5;
          if ( f == 'U' ) maxlen = 9;
          if ( len > maxlen ) len = maxlen;
          var span = new Span(start, len + 1);
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
