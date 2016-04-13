using System;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class BasicCStringScanner : IStringScanner {
    protected ITextChars text;
    public BasicCStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length - 1);
      // always skip the first char
      // (since quotes are included in the string)
      this.text.Next();
    }
    public StringPart? Next() {
      while ( !text.EndOfLine ) {
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
      var span = new Span(start, len + 1);
      return new StringPart(span, StringPartType.EscapeSequence);
    }
  }
}
