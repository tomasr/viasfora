using System;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class FSharpBraceScanner : IBraceScanner, IResumeControl {
    const int stText = 0;
    const int stString = 1;
    const int stChar = 2;
    const int stVerbatimString = 3;
    const int stMultiLineComment = 4;
    const int stTripleQuotedString = 5;
    private int status = stText;

    public String BraceList {
      get { return "(){}[]"; }
    }

    public FSharpBraceScanner() {
    }

    public void Reset(int state) {
      this.status = stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stChar: ParseCharLiteral(tc); break;
          case stMultiLineComment: ParseMultiLineComment(tc); break;
          case stVerbatimString: ParseVerbatimString(tc); break;
          case stTripleQuotedString: ParseTripleQuotedString(tc); break;
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    public bool CanResume(CharPos brace) {
      // When adding (*, we want to be able to ignore the
      // ( and go back to the previous brace
      return brace.Char != '(';
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        // multi-line comment
        if ( tc.Char() == '(' && tc.NChar() == '*' && tc.NNChar() != ')') {
          this.status = stMultiLineComment;
          tc.Skip(2);
          this.ParseMultiLineComment(tc);
        } else if ( tc.Char() == '/' && tc.NChar() == '/' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '@' && tc.NChar() == '"' ) {
          this.status = stVerbatimString;
          tc.Skip(2);
          this.ParseVerbatimString(tc);
        } else if ( tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"' ) {
          this.status = stTripleQuotedString;
          tc.Skip(3);
          this.ParseTripleQuotedString(tc);
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( tc.Char() == '<' && tc.NChar() == '\'') {
          // this is just a generic parameter, so skip it already
          tc.Skip(2);
        } else if ( Char.IsLetterOrDigit(tc.Char()) && tc.NChar() == '\'' ) {
          // identifier like c'
          tc.Skip(2);
        } else if ( tc.Char() == '\'' ) {
          this.status = stChar;
          tc.Next();
          this.ParseCharLiteral(tc);
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
      // valid:
      // - 'a'
      // - '\b'
      // - '\uaaaa'
      // - '()
      // not valid:
      // - 'a, 
      // - 'a 
      // - 'a) 
      // mark is just after the opening '
      if ( tc.Char() == '\\' ) {
        // skip until next quote
        tc.Skip(2);
        while ( !tc.EndOfLine && tc.Char() != '\'' ) {
          tc.Next();
        }
        tc.Next();
      } else {
        // skip the first char, as it's going to be a literal
        // however, if the next char isn't a ', assume
        // this is a generic declaration
        tc.Next();
        if ( tc.Char() == '\'' ) {
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
          this.status = stText;
          break;
        } else {
          tc.Next();
        }
      }
    }

    private void ParseVerbatimString(ITextChars tc) {
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

    private void ParseTripleQuotedString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"' ) {
          tc.Skip(3);
          this.status = stText;
          return;
        } else {
          tc.Next();
        }
      }
    }

    private void ParseMultiLineComment(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '*' && tc.NChar() == ')' ) {
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
