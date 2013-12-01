using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  class FSharp : LanguageInfo {
    public const String ContentType = "F#";
    static readonly String[] KEYWORDS = {
         "if", "then", "elif", "else", "match", "with",
         "for", "do", "to", "done", "while", "rec",
         "failwith", "yield"
      };
    static readonly String[] LINQ_KEYWORDS = {
         "query", "select", "seq"
      };
    static readonly String[] VIS_KEYWORDS = {
         "public", "private", "internal"
      };
    protected override String[] ControlFlowDefaults {
      get { return KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return VIS_KEYWORDS; }
    }
    protected override String KeyName {
      get { return "FSharp"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
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
      if ( pos > 0 && text[pos - 1] == '(' && text[pos] == '*' ) {
        return true;
      }
      return false;
    }
    public override bool IsMultiLineCommentEnd(string text, int pos) {
      if ( pos > 0 && text[pos - 1] == '*' && text[pos] == ')' ) {
        return true;
      }
      return false;
    }
    public override bool IsSingleLineStringStart(string text, int pos, out char quote) {
      // F# does not have single line strings, but we can use
      // this for char literals
      quote = '\'';
      if ( text[pos] == '\'' ) {
        // if this is <' this is very likely a generic parameter, so skip it
        if ( pos > 0 && text[pos - 1] == '<' )
          return false;
        // this may, or may not be, a char literal
        // so probe ahead. We're looking for:
        // 'c' or '\b' or '\uXXXX' or '\uXXXXXXXX' 
        if ( pos < text.Length - 2 ) {
          if ( text[pos + 1] == '\\' ) {
            // has to be a escape sequence
            return true;
          }
          if ( text[pos + 2] == '\'' )
            return true;
        }
      }
      return false;
    }
    public override bool IsMultiLineStringStart(string text, int pos, out char quote) {
      quote = '\0';
      if ( text[pos] == '"' ) {
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
  }
}
