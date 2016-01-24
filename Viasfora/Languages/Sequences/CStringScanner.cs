using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CStringScanner : IStringScanner {
    protected ITextChars text;
    public CStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length - 1);
      // If this is an at-string, or a C preprocessor include
      // skip it
      if ( this.text.Char() == '@' || this.text.Char() == '<' ) {
        this.text.SkipRemainder();
      } else {
        // always skip the first char
        // (since quotes are included in the string)
        this.text.Next();
      }
    }
    public StringPart? Next() {
      while ( !text.EndOfLine ) {
        if ( text.Char() == '\\' ) {
          return ParseEscapeSequence();
        } else if ( text.Char() == '%' ) {
          // skip %%
          if ( text.NChar() == '%' ) {
            text.Skip(2);
            continue;
          }
          StringPart part = new StringPart();
          if ( ParseFormatSpecifier(ref part) )
            return part;
        }
        text.Next();
      }
      return null;
    }

    private StringPart ParseEscapeSequence() {
      // text.Char() == \
      int start = text.Position;
      int len = 1;
      text.Next();

      int maxlen = Int32.MaxValue;

      char f = text.Char();
      text.Next();
      // not perfect, but close enough for first version
      if ( f == 'x' || f == 'X' || f == 'u' || f == 'U' ) {
        if ( f == 'u' ) maxlen = 5;
        else if ( f == 'U' ) maxlen = 9;

        while ( text.Char().IsHexDigit() && len < maxlen ) {
          text.Next();
          len++;
        }
      }
      var span = new Span(start, len + 1);
      return new StringPart(span, StringPartType.EscapeSequence);
    }

    private bool ParseFormatSpecifier(ref StringPart result) {
      // https://en.wikipedia.org/wiki/Printf_format_string#Syntax
      // %[parameter][flags][width][.precision][length]type

      // text.Char() == '%'
      int start = text.Position;
      text.Next(); // skip %
      int len = 1;
      while ( true ) {
        if ( text.EndOfLine || text.Char() == '\\' ) {
          break;
        }
        len++;
        if ( Char.IsLetter(text.Char()) ) {
          text.Next();
          break;
        }
        text.Next();
      }
      // if len == 1, then we found %"
      if ( len < 2 )
        return false;

      result = new StringPart(start, len, StringPartType.FormatSpecifier);
      return true;
    }
  }
}
