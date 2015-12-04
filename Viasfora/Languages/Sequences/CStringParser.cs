using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CStringParser : IStringParser {
    private String text;
    private int start;
    public CStringParser(String text) {
      this.text = text;
      // always skip the first char
      // (since quotes are included in the string)
      this.start = 1;
      // If this is an at-string, or a C preprocessor include
      // skip it
      if ( text.StartsWith("@") || text.StartsWith("<") )
        this.start = text.Length;
    }
    public StringPart? Next() {
      while ( start < text.Length - 2 ) {
        if ( text[start] == '\\' ) {
          return ParseEscapeSequence();
        } else if ( text[start] == '%' ) {
          // skip %%
          if ( start + 1 < text.Length && text[start + 1] == '%' ) {
            start += 2;
            continue;
          }
          return ParseFormatSpecifier();
        }
        start++;
      }
      return null;
    }

    private StringPart ParseEscapeSequence() {
      int len = 1;
      int maxlen = Int32.MaxValue;
      char f = text[start + 1];
      // not perfect, but close enough for first version
      if ( f == 'x' || f == 'X' || f == 'u' || f == 'U' ) {
        while ( (start + len) < text.Length && text[start + len + 1].IsHexDigit() ) {
          len++;
        }
      }
      if ( f == 'u' ) maxlen = 5;
      if ( f == 'U' ) maxlen = 9;
      if ( len > maxlen ) len = maxlen;
      var span = new Span(start, len + 1);
      start += len + 1;
      return new StringPart(span, StringPartType.EscapeSequence);
    }

    private StringPart ParseFormatSpecifier() {
      // text[start] == '%'
      int rs = start;
      int end = start + 1;
      for ( ; end < text.Length; end++ ) {
        // we're lazy, so don't bother explicitly handling
        // proper sequences
        if ( Char.IsLetter(text[end]) )
          break;
        // if we reach ", the string
        // ended and we can't parse anymore
        if ( text[end] == '\\' || text[end] == '\"' ) {
          end--;
          break;
        }
      }
      start = end + 1;
      return new StringPart(rs, end - rs + 1, StringPartType.FormatSpecifier);
    }

    protected void SetStart(int newStart) {
      this.start = newStart;
    }
  }
}
