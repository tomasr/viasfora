using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CSharpStringScanner : IStringScanner {
    private ITextChars text;
    private bool isInterpolated;

    public CSharpStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length - 1);
      // If this is an at-string, skip it
      if ( this.text.Char() == '@' ) {
        this.text.SkipRemainder();
      } else if ( this.text.Char() == '$' ) {
        this.isInterpolated = true;
        this.text.Skip(2);
      } else {
        // always skip the first char
        // (since quotes are included in the string)
        this.text.Next();
      }
    }
    public StringPart? Next() {
      while ( !text.EndOfLine ) {
        if ( text.Char() == '\\' ) {
          return BasicCStringScanner.ParseEscapeSequence(text);
        } else if ( text.Char() == '{' && text.NChar() == '{' ) {
          text.Next(); // skip it
        } else if ( text.Char() == '{' && !isInterpolated ) {
          StringPart part = new StringPart();
          if ( ParseFormatSpecifier(ref part) )
            return part;
        }
        text.Next();
      }
      return null;
    }
    private bool ParseFormatSpecifier(ref StringPart result) {
      // text.Char() == '{'
      int start = text.Position;
      int len = 1;
      text.Next();
      while ( !text.EndOfLine ) {
        len++;
        if ( text.Char() == '}' ) {
          result = new StringPart(start, len, StringPartType.FormatSpecifier);
          text.Next();
          return true;
        }
        text.Next();
      }
      return false;
    }
  }
}
