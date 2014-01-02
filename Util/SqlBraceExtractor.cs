using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Util {
  public class SqlBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stString = 1;
    const int stMultiLineComment = 4;
    private int status = stText;
    private LanguageInfo lang;

    public SqlBraceExtractor(LanguageInfo lang) {
      this.lang = lang;
    }

    public IEnumerable<CharPos> Extract(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
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
        } else if ( tc.Char() == '-' && tc.NChar() == '-' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '\'' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( lang.BraceList.IndexOf(tc.Char()) >= 0 ) {
          yield return new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
        } else {
          tc.Next();
        }
      }
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\'' && tc.NChar() == '\'' ) {
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
