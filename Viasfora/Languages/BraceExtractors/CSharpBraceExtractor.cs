using System;
using System.Linq;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceExtractors {
  public class CSharpBraceExtractor : IBraceExtractor, IResumeControl {
    const int stText = 0;
    const int stString = 1;
    const int stChar = 2;
    const int stMultiLineString = 3;
    const int stMultiLineComment = 4;
    const int stIString = 10;

    private int status = stText;
    private bool parsingExpression = false;

    public String BraceList {
      get { return "(){}[]"; }
    }

    public CSharpBraceExtractor() {
    }

    public void Reset(int state) {
      this.status = state & 0xFFFF;
      this.parsingExpression = (state & 0xFF000000) != 0;
    }

    public bool CanResume(CharPos brace) {
      return brace.State == stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stChar: ParseCharLiteral(tc); break;
          case stMultiLineComment: ParseMultiLineComment(tc); break;
          case stMultiLineString: ParseMultiLineString(tc); break;
          case stIString:
            return ParseInterpolatedString(tc, ref pos);
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        // multi-line comment
        if ( tc.Char() == '/' && tc.NChar() == '*' ) {
          this.status = stMultiLineComment;
          tc.Skip(2);
          this.ParseMultiLineComment(tc);
        } else if ( tc.Char() == '/' && tc.NChar() == '/' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '@' && tc.NChar() == '"' ) {
          this.status = stMultiLineString;
          tc.Skip(2);
          this.ParseMultiLineString(tc);
        } else if ( tc.Char() == '$' && tc.NChar() == '"' ) {
          // Roslyn interpolated string
          this.parsingExpression = false;
          this.status = stIString;
          tc.Skip(2);
          return this.ParseInterpolatedString(tc, ref pos);
        } else if ( tc.Char() == '$' && tc.NChar() == '@' && tc.NNChar() == '"' ) {
          this.status = stMultiLineString;
          tc.Skip(3);
          this.ParseMultiLineString(tc);
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( tc.Char() == '\'' ) {
          this.status = stString;
          tc.Next();
          this.ParseCharLiteral(tc);
        } else if ( this.BraceList.IndexOf(tc.Char()) >= 0 ) {
          pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
          tc.Next();
          return true;
        } else {
          tc.Next();
        }
      }
      return false;
    }

    private void ParseCharLiteral(ITextChars tc) {
      while ( !tc.EndOfLine ) {
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

    private void ParseString(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == '"' ) {
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
        if ( tc.Char() == '"' && tc.NChar() == '"' ) {
          // means a single embedded double quote
          tc.Skip(2);
        } else if ( tc.Char() == '"' ) {
          tc.Next();
          this.status = stText;
          return;
        } else {
          tc.Next();
        }
      }
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
    // C# 6.0 interpolated string support:
    // this is a hack. It will not handle all possible expressions
    // but will handle most basic stuff
    private bool ParseInterpolatedString(ITextChars tc, ref CharPos pos) {
      while ( !tc.EndOfLine ) {
        if ( parsingExpression ) {
          //
          // we're inside an interpolated section
          //
          if ( tc.Char() == '"' ) {
            // opening string
            tc.Next();
            this.ParseString(tc);
            this.status = stText;
          } else if ( tc.Char() == '\'' ) {
            tc.Next();
            ParseCharLiteral(tc);
            this.status = stText;
          } else if ( tc.Char() == '}' ) {
            // reached the end
            this.parsingExpression = false;
            pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
            tc.Next();
            return true;
          } else if ( BraceList.Contains(tc.Char()) ) {
            pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
            tc.Next();
            return true;
          } else {
            tc.Next();
          }
        } else {
          //
          // parsing the string part
          //
          if ( tc.Char() == '\\' ) {
            // skip over escape sequences
            tc.Skip(2);
          } else if ( tc.Char() == '{' && tc.NChar() == '{' ) {
            tc.Skip(2);
          } else if ( tc.Char() == '{' ) {
            this.parsingExpression = true;
            pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
            tc.Next();
            return true;
          } else if ( tc.Char() == '"' ) {
            // done parsing the interpolated string
            this.status = stText;
            tc.Next();
            break;
          } else {
            tc.Next();
          }
        }
      }
      return false;
    }

    private int EncodedState() {
      int encoded = status;
      if ( parsingExpression )
        encoded |= 0x04000000;
      return encoded;
    }
  }
}
