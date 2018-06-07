using System;
using System.Linq;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class JScriptBraceScanner : IBraceScanner, IResumeControl {
    const int stText = 0;
    const int stString = 1;
    const int stChar = 2;
    const int stRegex = 3;
    const int stMultiLineComment = 4;
    const int stIString = 5;
    private int status = stText;
    private int nestingLevel = 0;
    private bool parsingExpression = false;

    public String BraceList => "(){}[]";

    public JScriptBraceScanner() {
    }

    public void Reset(int state) {
      this.status = state & 0xFF;
      this.parsingExpression = (state & 0x08000000) != 0;
      this.nestingLevel = (state & 0xFF0000) >> 24;
    }

    public bool CanResume(CharPos brace) {
      return brace.State == stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stChar: ParseCharLiteral(tc); break;
          case stMultiLineComment: ParseMultiLineComment(tc); break;
          case stIString:
            if ( ParseInterpolatedString(tc, ref pos) ) {
              return true;
            }
            break;
          default:
            return ParseText(tc, ref pos);
        }
      }
      return false;
    }

    private bool ParseText(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        // multi-line comment
        if ( tc.Char() == '/' && tc.NChar() == '*' ) {
          this.status = stMultiLineComment;
          tc.Skip(2);
          this.ParseMultiLineComment(tc);
        } else if ( tc.Char() == '/' && tc.NChar() == '/' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '/' && CheckPrevious(tc.PreviousToken()) ) {
          // probably a regular expression literal
          tc.Next();
          this.status = stRegex;
          this.ParseRegex(tc);
        } else if ( tc.Char() == '"' ) {
          this.status = stString;
          tc.Next();
          this.ParseString(tc);
        } else if ( tc.Char() == '\'' ) {
          this.status = stString;
          tc.Next();
          this.ParseCharLiteral(tc);
        } else if ( tc.Char() == '`' ) {
          this.status = stIString;
          tc.Next();
          return this.ParseInterpolatedString(tc, ref pos);
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

    private bool CheckPrevious(String previous) {
      // javascript has a nasty syntax
      // bloody hack based on 
      // http://www-archive.mozilla.org/js/language/js20-2002-04/rationale/syntax.html#regular-expressions

      if ( String.IsNullOrEmpty(previous) ) {
        return true;
      }
      char last = previous[previous.Length - 1];
      return "(,=:[!&|?{};".Contains(last);
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

    private void ParseRegex(ITextChars tc) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '\\' ) {
          // skip over escape sequences
          tc.Skip(2);
        } else if ( tc.Char() == '/' ) {
          tc.Next();
          break;
        } else {
          tc.Next();
        }
      }
      this.status = stText;
    }

    private void ParseString(ITextChars tc) {
      while ( !tc.AtEnd ) {
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

    private void ParseMultiLineComment(ITextChars tc) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '*' && tc.NChar() == '/' ) {
          tc.Skip(2);
          this.status = stText;
          return;
        } else {
          tc.Next();
        }
      }
    }

    // template literal support, 
    // see https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Template_literals
    private bool ParseInterpolatedString(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        if ( this.parsingExpression ) {
          // inside template literal expression in ${}
          if ( ParseTemplateExpressionChar(tc, ref pos) )
            return true;
        } else {
          // in the string part
          if ( tc.Char() == '\\' ) {
            // skip over escape sequences
            tc.Skip(2);
          } else if ( tc.Char() == '$' && tc.NChar() == '{' ) {
            // opening expression
            this.parsingExpression = true;
            this.nestingLevel++;
            tc.Next(); // skip $
            pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
            tc.Next(); // skip {
            return true;
          } else if ( tc.Char() == '`' ) {
            // done parsing the template 
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

    private bool ParseTemplateExpressionChar(ITextChars tc, ref CharPos pos) {
      if ( tc.Char() == '"' ) {
        // opening string
        tc.Next();
        this.ParseString(tc);
        this.status = stIString;
      } else if ( tc.Char() == '\'' ) {
        tc.Next();
        ParseCharLiteral(tc);
        this.status = stIString;
      } else if ( tc.Char() == '}' ) {
        // reached the end
        this.nestingLevel--;
        if ( this.nestingLevel == 0 ) {
          this.parsingExpression = false;
        }
        pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
        tc.Next();
        return true;
      } else if ( BraceList.Contains(tc.Char()) ) {
        pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
        if ( tc.Char() == '{' )
          this.nestingLevel++;
        tc.Next();
        return true;
      } else {
        tc.Next();
      }
      return false;
    }

    private int EncodedState() {
      int encoded = this.status;
      if ( this.parsingExpression )
        encoded |= 0x08000000;
      encoded |= (this.nestingLevel & 0xFF) << 24;
      return encoded;
    }
  }
}
