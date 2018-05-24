using System;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public class SimpleTokenizer : ITokenizer {
    private ITextChars tc;
    private bool reachedEnd;
    private String currentToken;

    public bool AtEnd => this.reachedEnd;
    public String Token => this.currentToken;

    public SimpleTokenizer(String text) {
      this.tc = new StringChars(text);
    }

    public bool Next() {
      if ( this.tc.AtEnd ) {
        this.currentToken = "";
        this.reachedEnd = true;
       return false;
      }
      // skip whitespace
      while ( !this.tc.AtEnd && Char.IsWhiteSpace(this.tc.Char()) ) {
        this.tc.Next();
      }
      if ( this.tc.AtEnd ) {
        this.currentToken = "";
        this.reachedEnd = true;
        return false;
      }
      // if it's a special character, return it on its own
      if ( !Char.IsLetterOrDigit(this.tc.Char()) ) {
        char ch = this.tc.Char();
        this.tc.Next();
        this.currentToken = "" + ch;
        return true;
      }
      // return the word
      StringBuilder sb = new StringBuilder();
      while ( !this.tc.AtEnd ) {
        char ch = this.tc.Char();
        if ( !Char.IsLetterOrDigit(ch) ) break;
        this.tc.Next();
        sb.Append(ch);
      }
      this.currentToken = sb.ToString();
      return true;
    }
  }
}
