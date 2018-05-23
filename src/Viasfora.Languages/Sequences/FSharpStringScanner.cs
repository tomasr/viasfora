using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class FSharpStringScanner : IStringScanner {
    private ITextChars text;
    private const String escapeChar = "\"\\'ntbrafv";

    public FSharpStringScanner(String theText) {
      this.text = new StringChars(theText);

      if ( theText.StartsWith("\"\"\"") || theText.StartsWith("@") ) {
        this.text.SkipRemainder();
      }
      // always skip the first char
      // (since quotes are included in the string)
      this.text.Next();
    }
    public StringPart? Next() {
      while ( !this.text.AtEnd ) {
        if ( this.text.Char() == '\\' ) {
          StringPart part = new StringPart();
          if ( TryParseEscapeSequence(ref part) )
            return part;
        } else if ( this.text.Char() == '%' ) {
          StringPart part = new StringPart();
          if ( TryParseFormatSpecifier(ref part) )
            return part;
        } else {
          this.text.Next();
        }
      }
      return null;
    }

    private bool TryParseEscapeSequence(ref StringPart part) {
      int start = this.text.Position;
      this.text.Next();
      if ( escapeChar.IndexOf(this.text.Char()) >= 0 ) {
        this.text.Next();
        part = new StringPart(new TextSpan(this.text.Position - 2, 2));
        return true;
      }
      if ( Char.IsDigit(this.text.Char()) && Char.IsDigit(this.text.NChar()) && Char.IsDigit(this.text.NNChar()) ) {
        // a trigraph
        this.text.Skip(3);
        part = new StringPart(new TextSpan(this.text.Position - 4, 4));
        return true;
      }
      if ( this.text.Char() == '0' && !Char.IsDigit(this.text.NChar()) ) {
        // \0
        this.text.Next();
        part = new StringPart(new TextSpan(this.text.Position - 2, 2));
        return true;
      }
      if ( this.text.Char() == 'u' ) {
        this.text.Next();
        this.text.Mark();
        TextSpan? span = TryParseShortUnicode();
        if ( span.HasValue ) {
          part = new StringPart(span.Value);
          return true;
        }
        this.text.BackToMark();
      }
      if ( this.text.Char() == 'U' ) {
        this.text.Next();
        this.text.Mark();
        TextSpan? span = TryParseLongUnicode();
        if ( span.HasValue ) {
          part = new StringPart(span.Value);
          return true;
        }
        this.text.BackToMark();
      }
      // unrecognized sequence, return it as error
      this.text.Next();
      int length = this.text.Position - start;
      part = new StringPart(new TextSpan(start, length), StringPartType.EscapeSequenceError);
      return true;
    }

    private TextSpan? TryParseShortUnicode() {
      // \Uhhhhhhhh
      for ( int i = 0; i < 4; i++ ) {
        if ( !this.text.Char().IsHexDigit() ) {
          return null;
        }
        this.text.Next();
      }
      return new TextSpan(this.text.Position - 6, 6);
    }
    private TextSpan? TryParseLongUnicode() {
      // \Uhhhhhhhh
      for ( int i = 0; i < 8; i++ ) {
        if ( !this.text.Char().IsHexDigit() ) {
          return null;
        }
        this.text.Next();
      }
      return new TextSpan(this.text.Position - 10, 10);
    }

    private bool TryParseFormatSpecifier(ref StringPart result) {
      // https://msdn.microsoft.com/en-us/library/ee370560.aspx
      // %[flags][width][.precision]type
      int start = this.text.Position;
      this.text.Next(); // skip '%'
      // ignore %%
      if ( this.text.Char() == '%' ) {
        this.text.Next();
        return false;
      }
      // ignore EOF
      if ( this.text.AtEnd || this.text.Char() == '\"' )
        return false;

      int len = 1;
      while ( !this.text.AtEnd ) {
        len++;
        if ( Char.IsLetter(this.text.Char()) ) {
          this.text.Next();
          break;
        }
        this.text.Next();
      }
      result = new StringPart(start, len, StringPartType.FormatSpecifier);
      return true;
    }
  }
}
