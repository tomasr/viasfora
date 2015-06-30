using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceExtractors {
  public class PythonBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stString = 1;
    const int stMultiLineString = 3;
    private int status = stText;
    private char quoteChar;

    public String BraceList {
      get { return "(){}[]"; }
    }

    public int ReparseWindow {
      get { return 0; }
    }

    public PythonBraceExtractor() {
    }

    public void Reset() {
      this.status = stText;
    }

    public IEnumerable<CharPos> Extract(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stMultiLineString: ParseMultiLineString(tc); break;
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
        if ( tc.Char() == '#' ) {
          tc.SkipRemainder();
        } else if ( (tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"') 
                 || (tc.Char() == '\'' && tc.NChar() == '\'' && tc.NNChar() == '\'') ) {
          this.status = stMultiLineString;
          this.quoteChar = tc.Char();
          tc.Skip(3);
          this.ParseMultiLineString(tc);
        } else if ( tc.Char() == '\'' || tc.Char() == '"' ) {
          this.status = stString;
          this.quoteChar = tc.Char();
          tc.Next();
          this.ParseString(tc);
        } else if ( this.BraceList.IndexOf(tc.Char()) >= 0 ) {
          yield return new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
        } else {
          tc.Next();
        }
      }
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == quoteChar ) {
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
      this.status = stText;
    }

    private void ParseMultiLineString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '"' ) {
          tc.Next();
          this.status = stText;
          return;
        } else {
          tc.Next();
        }
      }
    }
  }
}
