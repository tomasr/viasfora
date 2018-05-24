using System;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class VbBraceScanner : IBraceScanner {
    const int stText = 0;
    const int stString = 1;
    private int status;

    public String BraceList {
      get { return "(){}"; }
    }

    public VbBraceScanner() {
      this.status = stText;
    }

    public void Reset(int state) {
      this.status = stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      pos = CharPos.Empty;
      while ( !tc.AtEnd ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '\'' ) {
          // single line comment
          tc.SkipRemainder();
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( this.BraceList.IndexOf(tc.Char()) >= 0 ) {
          pos = new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
          return true;
        } else {
          tc.Next();
        }
      }
      return false;
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.AtEnd ) {
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
