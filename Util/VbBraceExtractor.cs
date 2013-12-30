using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;

namespace Winterdom.Viasfora.Util {
  public class VbBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stString = 1;
    private int status;
    private LanguageInfo language;

    public VbBraceExtractor(LanguageInfo lang) {
      this.language = lang;
      this.status = stText;
    }

    public IEnumerable<CharPos> Extract(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
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
        if ( tc.Char() == '\'' ) {
          // single line comment
          tc.SkipRemainder();
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( language.BraceList.Contains(tc.Char()) ) {
          yield return new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
        } else {
          tc.Next();
        }
      }
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '"' && tc.NChar() == '"' ) {
          // embedded quotes, skip
          tc.Skip(2);
        } else if ( tc.Char() == '"' ) {
          this.status = stText;
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
    }
  }
}
