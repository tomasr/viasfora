using System;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class MplBraceScanner : IBraceScanner {
    private enum State {
      Text, MultiLineString
    }

    private State status = State.Text;

    public String BraceList => "(){}[]:;";

    public MplBraceScanner() {
    }

    public void Reset(int state) {
      this.status = (int)State.Text;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        switch ( this.status ) {
          case State.MultiLineString: String(tc); break;
          default:
            return Parse(tc, ref pos);
        }
      }
      return false;
    }

    private bool Parse(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        // Comment.
        if ( tc.Char() == '#' ) {
          tc.SkipRemainder();
        }

        // String.
        else if ( tc.Char() == '"' ) {
          tc.Next();

          this.status = State.MultiLineString;
          this.String(tc);

          continue;
        }

        // Braces.
        else if ( this.BraceList.IndexOf(tc.Char()) >= 0 ) {
          pos = new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
          return true;
        }

        // Code.
        tc.Next();
      }
      return false;
    }

    private void String(ITextChars tc) {
      while ( !tc.AtEnd ) {
        // End of a String.
        if ( tc.Char() == '"' ) {
          tc.Next();
          this.status = State.Text;
          return;
        }

        // Start of an Escape Sequence.
        if ( tc.Char() == '\\' )
          tc.Next();

        // Content of a String, or an Escaped Character.
        tc.Next();
      }
    }
  }
}
