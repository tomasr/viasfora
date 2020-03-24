using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class MplStringScanner : IStringScanner {
    protected ITextChars text;
    public MplStringScanner(String text) {
      this.text = new StringChars(text, 0, text.Length - 1);
    }
    public StringPart? Next() {
      while ( !this.text.AtEnd ) {
        if ( this.text.Char() == '\\' ) {
          return ParseEscapeSequence(this.text);
        }
        this.text.Next();
      }
      return null;
    }

    private StringPart? ParseEscapeSequence(ITextChars text) {
      // text.Char() == \
      int start = text.Position;
      text.Next();

      char f = text.Char();
      text.Next();
      if ( f == '\\' || f == '\"' ) {
        var span = new TextSpan(start, 2);
        return new StringPart(span, StringPartType.EscapeSequence);
      }
      return null;
    }
  }
}
