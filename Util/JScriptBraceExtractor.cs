using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Util {
  public class JScriptBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stString = 1;
    const int stChar = 2;
    const int stRegex = 3;
    const int stMultiLineComment = 4;
    private int status = stText;
    private LanguageInfo lang;

    public JScriptBraceExtractor(LanguageInfo lang) {
      this.lang = lang;
    }

    public IEnumerable<CharPos> Extract(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stChar: ParseCharLiteral(tc); break;
          case stMultiLineComment: ParseMultiLineComment(tc); break;
          default: 
            foreach ( var p in ParseText(tc) ) {
              yield return p;
            }
            break;
        }
      }
    }

    private IEnumerable<CharPos> ParseText(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        // multi-line comment
        if ( tc.Char() == '/' && tc.NChar() == '*' ) {
          this.status = stMultiLineComment;
          tc.Skip(2);
          this.ParseMultiLineComment(tc);
        } else if ( tc.Char() == '/' && tc.NChar() == '/' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '/' && CheckPrevious(tc.PreviousToken()) ) {
          // probably a regular expression literal
          tc.Next();
          this.status = stRegex;
          this.ParseRegex(tc);
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( tc.Char() == '\'' ) {
          this.status = stString;
          tc.Next();
          this.ParseCharLiteral(tc);
        } else if ( lang.BraceList.Contains(tc.Char()) ) {
          yield return new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
        } else {
          tc.Next();
        }
      }
    }

    private bool CheckPrevious(String previous) {
      // javascript has a nasty syntax
      // bloody hack based on 
      // http://www-archive.mozilla.org/js/language/js20-2002-04/rationale/syntax.html#regular-expressions

      if ( String.IsNullOrEmpty(previous) ) {
        return true;
      }
      char last = previous[previous.Length - 1];
      return "(,=:[!&|?{};".Contains(last);
    }

    private void ParseCharLiteral(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == '\'' ) {
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
      this.status = stText;
    }

    private void ParseRegex(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == '/' ) {
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
      this.status = stText;
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == '"' ) {
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
      this.status = stText;
    }

    private void ParseMultiLineComment(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '*' && tc.NChar() == '/' ) {
          tc.Skip(2);
          this.status = stText;
          return;
        } else {
          tc.Next();
        }
      }
    }
  }
}
