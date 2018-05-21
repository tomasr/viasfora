using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class BasicCStringScanner : IStringScanner {
    protected ITextChars text;
    public BasicCStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length);
      // only skip the first char if it's a quote
      // vs2017 does not can break the token and parse it in partial chunks
      if ( this.text.Char() == '\'' || this.text.Char() == '"' ) {
        this.text.Next();
      }
    }
    public StringPart? Next() {
      while ( !text.AtEnd ) {
        if ( text.Char() == '\\' ) {
          return ParseEscapeSequence(text);
        }
        text.Next();
      }
      return null;
    }

    internal static StringPart ParseEscapeSequence(ITextChars text) {
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
      var span = new TextSpan(start, len + 1);
      return new StringPart(span, StringPartType.EscapeSequence);
    }
  }
}
