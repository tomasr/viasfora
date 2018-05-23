using System;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class USqlBraceScanner : IBraceScanner {
    const int stText = 0;
    const int stString = 1;
    const int stMultiLineComment = 4;
    private int status = stText;

    public string BraceList {
      get { return "()[]{}"; }
    }

    public void Reset(int state) {
      this.status = stText;
    }
    public bool Extract(ITextChars tc, ref CharPos pos) {
      pos = CharPos.Empty;
      while ( !tc.AtEnd ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stMultiLineComment: ParseMultilineComment(tc); break;
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '/' && tc.NChar() == '*' ) {
          tc.Skip(2);
          this.status = stMultiLineComment;
          ParseMultilineComment(tc);
        } else if ( tc.Char() == '/' && tc.NChar() == '/' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '\'' ) {
          this.status = stString;
          tc.Next();
          ParseCharLiteral(tc);
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          ParseString(tc);
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

    private void ParseCharLiteral(ITextChars tc) {
      while ( !tc.AtEnd ) {
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

    private void ParseMultilineComment(ITextChars tc) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '*' && tc.Char() == '/' ) {
          tc.Skip(2);
          this.status = stText;
          break;
        } else {
          tc.Next();
        }
      }
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '\"' ) {
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
