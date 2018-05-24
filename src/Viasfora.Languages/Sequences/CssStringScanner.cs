using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class CssStringScanner : IStringScanner {
    private String text;
    private int start;
    public CssStringScanner(String text) {
      this.text = text;
      // always skip the first char
      // (since quotes are included in the string)
      this.start = 1;
    }
    public StringPart? Next() {
      const int maxHexLen = 6;
      while ( this.start < this.text.Length - 2 ) {
        if ( this.text[this.start] == '\\' ) {
          int len = 1;
          while ( (this.start+len) < this.text.Length && this.text[this.start+len+1].IsHexDigit() && len < maxHexLen ) {
            len++;
          }

          var span = new TextSpan(this.start, len+1);
          this.start += len + 1;
          return new StringPart(span, StringPartType.EscapeSequence);
        }
        this.start++;
      }
      return null;
    }
  }
}
