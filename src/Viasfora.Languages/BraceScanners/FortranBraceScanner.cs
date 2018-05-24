using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class FortranBraceScanner : IBraceScanner {
    const int stText = 0;
    const int stStringSingle = 1;
    const int stStringDouble = 2;
    private int status = stText;

    public string BraceList => "()";

    public void Reset(int state) {
      this.status = stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        switch ( this.status ) {
          case stStringSingle: ParseStringSingle(tc); break;
          case stStringDouble: ParseStringDouble(tc); break;
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '!' ) {
          // single line comment
          tc.SkipRemainder();
        } else if ( tc.Char() == '\'' ) {
          this.status = stStringSingle;
          tc.Next();
          ParseStringSingle(tc);
        } else if ( tc.Char() == '"' ) {
          this.status = stStringDouble;
          tc.Next();
          ParseStringDouble(tc);
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

    private void ParseStringSingle(ITextChars tc) => ParseString(tc, '\'');
    private void ParseStringDouble(ITextChars tc) => ParseString(tc, '"');

    private void ParseString(ITextChars tc, char quote) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == quote && tc.NChar() == quote ) {
          // double quote, meaning a single literal quote, skip
          tc.Skip(2);
        } else if ( tc.Char() == quote ) {
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
      this.status = stText;
    }
  }
}