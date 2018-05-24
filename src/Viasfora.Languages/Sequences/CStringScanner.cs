using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CStringScanner : IStringScanner {
    protected ITextChars text;
    private bool isRString;

    public CStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length - 1);
      // If it's a C preprocessor include
      // skip it
      if ( this.text.Char() == '<' ) {
        this.text.SkipRemainder();
      } else if ( this.text.Char() == 'R' ) {
        this.isRString = true;
        this.text.Skip(2);
      } else {
        // always skip the first char
        // (since quotes are included in the string)
        this.text.Next();
      }
    }
    public StringPart? Next() {
      while ( !this.text.AtEnd ) {
        if ( this.text.Char() == '\\' && !this.isRString ) {
          return BasicCStringScanner.ParseEscapeSequence(this.text);
        } else if ( this.text.Char() == '%' ) {
          // skip %%
          if ( this.text.NChar() == '%' ) {
            this.text.Skip(2);
            continue;
          }
          StringPart part = new StringPart();
          if ( ParseFormatSpecifier(ref part) )
            return part;
        }
        this.text.Next();
      }
      return null;
    }
    private bool ParseFormatSpecifier(ref StringPart result) {
      // https://en.wikipedia.org/wiki/Printf_format_string#Syntax
      // %[parameter][flags][width][.precision][length]type

      // text.Char() == '%'
      int start = this.text.Position;
      this.text.Next(); // skip %
      int len = 1;
      while ( true ) {
        if ( this.text.AtEnd || this.text.Char() == '\\' ) {
          break;
        }
        len++;
        if ( Char.IsLetter(this.text.Char()) ) {
          this.text.Next();
          break;
        }
        this.text.Next();
      }
      // if len == 1, then we found %"
      if ( len < 2 )
        return false;

      result = new StringPart(start, len, StringPartType.FormatSpecifier);
      return true;
    }
  }
}
