using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CSharpStringScanner : IStringScanner {
    private ITextChars text;
    private bool isInterpolated;
    private bool isVerbatim;

    public CSharpStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length - 1);
      // If this is an at-string, skip it
      char first = this.text.Char();
      if ( first == '@' ) {
        this.isVerbatim = true;
        this.text.Skip(2);
      } else if ( first == '$' ) {
        this.isInterpolated = true;
        if ( this.text.NChar() == '@' ) {
          this.isVerbatim = true;
          this.text.Skip(3);
        } else {
          this.text.Skip(2);
        }
      } else if ( first == '"' || first == '\'' ) {
        // always skip the first char
        // (since quotes are included in the string)
        this.text.Next();
      }
    }
    public StringPart? Next() {
      while ( !text.EndOfLine ) {
        if ( text.Char() == '\\' && !this.isVerbatim ) {
          return BasicCStringScanner.ParseEscapeSequence(text);
        } else if ( text.Char() == '{' && text.NChar() == '{' ) {
          text.Next(); // skip it
        } else if ( text.Char() == '{' && !this.isInterpolated ) {
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
