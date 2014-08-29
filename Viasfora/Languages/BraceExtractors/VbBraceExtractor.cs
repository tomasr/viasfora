using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceExtractors {
  public class VbBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stString = 1;
    private int status;
    private String braceList;

    public VbBraceExtractor(String braces) {
      this.braceList = braces;
      this.status = stText;
    }

    public void Reset() {
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
        } else if ( braceList.IndexOf(tc.Char()) >= 0 ) {
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
