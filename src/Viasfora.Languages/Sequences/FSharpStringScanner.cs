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
      while ( !text.EndOfLine ) {
        if ( text.Char() == '\\' ) {
          StringPart part = new StringPart();
          if ( TryParseEscapeSequence(ref part) )
            return part;
        } else if ( text.Char() == '%' ) {
          StringPart part = new StringPart();
          if ( TryParseFormatSpecifier(ref part) )
            return part;
        } else {
          text.Next();
        }
      }
      return null;
    }

    private bool TryParseEscapeSequence(ref StringPart part) {
      int start = this.text.Position;
      text.Next();
      if ( escapeChar.IndexOf(text.Char()) >= 0 ) {
        text.Next();
        part = new StringPart(new TextSpan(text.Position - 2, 2));
        return true;
      }
      if ( Char.IsDigit(text.Char()) && Char.IsDigit(text.NChar()) && Char.IsDigit(text.NNChar()) ) {
        // a trigraph
        text.Skip(3);
        part = new StringPart(new TextSpan(text.Position - 4, 4));
        return true;
      }
      if ( text.Char() == '0' && !Char.IsDigit(text.NChar()) ) {
        // \0
        text.Next();
        part = new StringPart(new TextSpan(text.Position - 2, 2));
        return true;
      }
      if ( text.Char() == 'u' ) {
        text.Next();
        text.Mark();
        TextSpan? span = TryParseShortUnicode();
        if ( span.HasValue ) {
          part = new StringPart(span.Value);
          return true;
        }
        text.BackToMark();
      }
      if ( text.Char() == 'U' ) {
        text.Next();
        text.Mark();
        TextSpan? span = TryParseLongUnicode();
        if ( span.HasValue ) {
          part = new StringPart(span.Value);
          return true;
        }
        text.BackToMark();
      }
      // unrecognized sequence, return it as error
      text.Next();
      int length = text.Position - start;
      part = new StringPart(new TextSpan(start, length), StringPartType.EscapeSequenceError);
      return true;
    }

    private TextSpan? TryParseShortUnicode() {
      // \Uhhhhhhhh
      for ( int i = 0; i < 4; i++ ) {
        if ( !text.Char().IsHexDigit() ) {
          return null;
        }
        text.Next();
      }
      return new TextSpan(text.Position - 6, 6);
    }
    private TextSpan? TryParseLongUnicode() {
      // \Uhhhhhhhh
      for ( int i = 0; i < 8; i++ ) {
        if ( !text.Char().IsHexDigit() ) {
          return null;
        }
        text.Next();
      }
      return new TextSpan(text.Position - 10, 10);
    }

    private bool TryParseFormatSpecifier(ref StringPart result) {
      // https://msdn.microsoft.com/en-us/library/ee370560.aspx
      // %[flags][width][.precision]type
      int start = text.Position;
      text.Next(); // skip '%'
      // ignore %%
      if ( text.Char() == '%' ) {
        text.Next();
        return false;
      }
      // ignore EOF
      if ( text.EndOfLine || text.Char() == '\"' )
        return false;

      int len = 1;
      while ( !text.EndOfLine ) {
        len++;
        if ( Char.IsLetter(text.Char()) ) {
          text.Next();
          break;
        }
        text.Next();
      }
      result = new StringPart(start, len, StringPartType.FormatSpecifier);
      return true;
    }
  }
}
