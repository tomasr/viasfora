using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceExtractors {
  public class FSharpBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stString = 1;
    const int stChar = 2;
    const int stMultiLineString = 3;
    const int stMultiLineComment = 4;
    const int stTripleQuotedString = 5;
    private int status = stText;
    private LanguageInfo lang;

    public FSharpBraceExtractor(LanguageInfo lang) {
      this.lang = lang;
    }

    public void Reset() {
      this.status = stText;
    }

    public IEnumerable<CharPos> Extract(ITextChars tc) {
      while ( !tc.EndOfLine ) {
        switch ( this.status ) {
          case stString: ParseString(tc); break;
          case stChar: ParseCharLiteral(tc); break;
          case stMultiLineComment: ParseMultiLineComment(tc); break;
          case stMultiLineString: ParseMultiLineString(tc); break;
          case stTripleQuotedString: ParseTripleQuotedString(tc); break;
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
        // multi-line comment
        if ( tc.Char() == '(' && tc.NChar() == '*' ) {
          this.status = stMultiLineComment;
          tc.Skip(2);
          this.ParseMultiLineComment(tc);
        } else if ( tc.Char() == '/' && tc.NChar() == '/' ) {
          tc.SkipRemainder();
        } else if ( tc.Char() == '@' && tc.NChar() == '"' ) {
          this.status = stMultiLineString;
          tc.Skip(2);
          this.ParseMultiLineString(tc);
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
        } else if ( (tc.Char() == '\'' && tc.NChar() == '\\') 
                 || (tc.Char() == '\'' && tc.NNChar() == '\'') ) {
          // char literal
          this.status = stString;
          tc.Next();
          this.ParseCharLiteral(tc);
        } else if ( lang.BraceList.IndexOf(tc.Char()) >= 0 ) {
          yield return new CharPos(tc.Char(), tc.AbsolutePosition);
          tc.Next();
        } else {
          tc.Next();
        }
      }
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
