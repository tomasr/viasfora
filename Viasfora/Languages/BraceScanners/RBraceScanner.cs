using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class RBraceScanner : IBraceScanner {
    const int stText = 0;
    const int stString = 1;
    const int stSQString = 2;
    private int status = stText;

    public String BraceList {
      get { return "{}()[]"; }
    }

    public void Reset(int state) {
      this.status = stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stSQString: ParseSQString(tc); break;
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        // multi-line comment
        if ( tc.Char() == '#' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( tc.Char() == '\'' ) {
          this.status = stSQString;
          tc.Next();
          this.ParseSQString(tc);
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
      ParseStringInt(tc, '"');
    }
    private void ParseSQString(ITextChars tc) {
      ParseStringInt(tc, '\'');
    }
    private void ParseStringInt(ITextChars tc, char quote) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == quote ) {
          tc.Next();
          this.status = stText;
          break;
        } else {
          tc.Next();
        }
      }
    }
  }
}
