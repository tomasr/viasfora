using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  abstract class CBasedLanguage : LanguageInfo {
    public override string BraceList {
      get { return "(){}[]"; }
    }

    public override bool IsSingleLineCommentStart(string text, int pos) {
      if ( pos > 0 && text[pos - 1] == '/' && text[pos] == '/' ) {
        return true;
      }
      return false;
    }
    public override bool IsMultiLineCommentStart(string text, int pos) {
      if ( pos > 0 && text[pos - 1] == '/' && text[pos] == '*' ) {
        return true;
      }
      return false;
    }
    public override bool IsMultiLineCommentEnd(string text, int pos) {
      if ( pos > 0 && text[pos - 1] == '*' && text[pos] == '/' ) {
        return true;
      }
      return false;
    }
    public override bool IsSingleLineStringStart(string text, int pos, out char quote) {
      quote = '\0';
      if ( IsQuote(text[pos]) ) {
        quote = text[pos];
        return true;
      }
      return false;
    }
    public override bool IsMultiLineStringStart(string text, int pos, out char quote) {
      quote = '\0';
      if ( pos > 0 && text[pos - 1] == '@' && IsQuote(text[pos])) {
        quote = text[pos];
        return true;
      }
      return false;
    }
    public override bool IsStringEnd(string text, int pos, char quote) {
      if ( text[pos] == quote ) {
        // check the character isn't escaped
        if ( pos > 0 && text[pos-1] == '\\' ) {
          // check the \ isn't escaped itself!
          if ( pos > 1 && text[pos - 2] == '\\' ) {
            return true;
          }
          return false;
        }
        return true;
      }
      return false;
    }

    private bool IsQuote(char ch) {
      return ch == '\'' || ch == '"';
    }
  }

}
