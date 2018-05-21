using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CSharpStringScanner : IStringScanner {
    private ITextChars text;
    private bool isInterpolated;
    private bool isVerbatim;

    public CSharpStringScanner(String text, String classificationName = "string") {
      this.text = new StringChars(text, 0, text.Length - 1);
      this.isVerbatim = classificationName == "string - verbatim";
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
      while ( !this.text.AtEnd ) {
        if ( this.text.Char() == '\\' && !this.isVerbatim ) {
          return BasicCStringScanner.ParseEscapeSequence(this.text);
        } else if ( this.text.Char() == '{' && this.text.NChar() == '{' ) {
          this.text.Next(); // skip it
        } else if ( this.text.Char() == '{' && !this.isInterpolated ) {
          StringPart part = new StringPart();
          if ( ParseFormatSpecifier(ref part) )
            return part;
        }
        this.text.Next();
      }
      return null;
    }
    private bool ParseFormatSpecifier(ref StringPart result) {
      // text.Char() == '{'
      int start = this.text.Position;
      int len = 1;
      this.text.Next();
      while ( !this.text.AtEnd ) {
        char ch = this.text.Char();
        if ( Char.IsWhiteSpace(ch) || ch.IsEndOfLine() )
          break;
        len++;
        if ( ch == '}' ) {
          result = new StringPart(start, len, StringPartType.FormatSpecifier);
          this.text.Next();
          return true;
        }
        this.text.Next();
      }
      return false;
    }
  }
}
