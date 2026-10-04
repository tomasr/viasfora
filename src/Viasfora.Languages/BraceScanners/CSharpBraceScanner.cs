using System;
using System.Collections.Generic;
using System.Linq;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class CSharpBraceScanner : IBraceScanner, IResumeControl {
    const int stText = 0;
    const int stString = 1;
    const int stChar = 2;
    const int stMultiLineComment = 4;
    const int stIString = 5;

    const int RawQuotesShift = 16;
    const int DollarsShift = 21;
    const int MaxEncodedCount = 0x1F;
    const int VerbatimFlag = 0x04000000;
    const int ParsingExpressionFlag = 0x08000000;

    private int status = stText;
    private bool multiLine = false;
    private int rawStringQuotes = 0;
    private readonly Stack<InterpolatedString> istrings = new Stack<InterpolatedString>();

    public String BraceList => "(){}[]";

    public CSharpBraceScanner() {
    }

    public void Reset(int state) {
      this.status = state & 0xFF;
      this.multiLine = false;
      this.rawStringQuotes = 0;
      this.istrings.Clear();
      // only the innermost interpolated string can be restored
      if ( this.status == stIString ) {
        this.istrings.Push(new InterpolatedString {
          NestingLevel = (state >> 8) & 0xFF,
          RawQuotes = (state >> RawQuotesShift) & MaxEncodedCount,
          Dollars = (state >> DollarsShift) & MaxEncodedCount,
          Verbatim = (state & VerbatimFlag) != 0,
          ParsingExpression = (state & ParsingExpressionFlag) != 0
        });
      }
    }

    public bool CanResume(CharPos brace) {
      return brace.State == stText;
    }

    public bool Extract(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd ) {
        switch ( this.status ) {
          case stString:
            if ( this.rawStringQuotes > 0 ) {
              ParseRawString(tc);
            } else if ( this.multiLine ) {
              ParseMultiLineString(tc);
            } else {
              ParseString(tc);
            }
            break;
          case stChar: ParseCharLiteral(tc); break;
          case stMultiLineComment: ParseMultiLineComment(tc); break;
          case stIString:
            if ( ParseInterpolatedString(tc, ref pos) )
              return true;
            break;
          default:
            if ( ParseText(tc, ref pos) )
              return true;
            break;
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
        } else if ( tc.Char() == '@' && tc.NChar() == '"' ) {
          this.status = stString;
          this.multiLine = true;
          tc.Skip(2);
          this.ParseMultiLineString(tc);
        } else if ( TryStartInterpolatedString(tc) ) {
          return this.ParseInterpolatedString(tc, ref pos);
        } else if ( tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"' ) {
          this.status = stString;
          this.rawStringQuotes = SkipQuotes(tc);
          this.ParseRawString(tc);
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

    // C# 11 raw string literal: closed by a run of quotes
    // at least as long as the opening one
    private void ParseRawString(ITextChars tc) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '"' ) {
          if ( SkipQuotes(tc) >= this.rawStringQuotes ) {
            this.status = stText;
            this.rawStringQuotes = 0;
            return;
          }
        } else {
          tc.Next();
        }
      }
    }

    private static int SkipQuotes(ITextChars tc) {
      return SkipAll(tc, '"');
    }

    private static int SkipAll(ITextChars tc, char ch) {
      int count = 0;
      while ( !tc.AtEnd && tc.Char() == ch ) {
        tc.Next();
        count++;
      }
      return count;
    }
    private void ParseMultiLineString(ITextChars tc) {
      while ( !tc.AtEnd ) {
        if ( tc.Char() == '"' && tc.NChar() == '"' ) {
          // means a single embedded double quote
          tc.Skip(2);
        } else if ( tc.Char() == '"' ) {
          tc.Next();
          this.status = stText;
          this.multiLine = false;
          return;
        } else {
          tc.Next();
        }
      }
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

    // Consumes the opening delimiter and returns true
    // if tc is at the start of an interpolated string
    private bool TryStartInterpolatedString(ITextChars tc) {
      if ( tc.Char() != '$' )
        return false;
      tc.Mark();
      var istring = new InterpolatedString { Dollars = SkipAll(tc, '$') };
      if ( tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"' ) {
        // C# 11 interpolated raw string
        istring.RawQuotes = SkipQuotes(tc);
      } else if ( istring.Dollars == 1 && tc.Char() == '"' ) {
        tc.Next();
      } else if ( istring.Dollars == 1 && tc.Char() == '@' && tc.NChar() == '"' ) {
        istring.Verbatim = true;
        tc.Skip(2);
      } else {
        tc.BackToMark();
        return false;
      }
      tc.ClearMark();
      this.istrings.Push(istring);
      this.status = stIString;
      return true;
    }

    // C# 6.0 interpolated string support:
    // this is a hack. It will not handle all possible expressions
    // but will handle most basic stuff
    private bool ParseInterpolatedString(ITextChars tc, ref CharPos pos) {
      while ( !tc.AtEnd && this.status == stIString ) {
        var istring = this.istrings.Peek();
        bool found;
        if ( istring.ParsingExpression ) {
          found = ParseInterpolationExpression(tc, istring, ref pos);
        } else if ( istring.IsRaw ) {
          found = ParseInterpolatedRawStringText(tc, istring, ref pos);
        } else {
          found = ParseInterpolatedStringText(tc, istring, ref pos);
        }
        if ( found )
          return true;
      }
      return false;
    }

    // we're inside an interpolated section
    private bool ParseInterpolationExpression(ITextChars tc, InterpolatedString istring, ref CharPos pos) {
      if ( TryStartInterpolatedString(tc) ) {
        // opening nested interpolated string
      } else if ( tc.Char() == '@' && tc.NChar() == '"' ) {
        // opening nested verbatim string
        tc.Skip(2);
        this.ParseMultiLineString(tc);
        this.status = stIString;
      } else if ( tc.Char() == '"' ) {
        // opening string
        tc.Next();
        this.ParseString(tc);
        this.status = stIString;
      } else if ( tc.Char() == '\'' ) {
        tc.Next();
        ParseCharLiteral(tc);
        this.status = stIString;
      } else if ( tc.Char() == '}' ) {
        pos = ParseClosingBrace(tc, istring);
        return true;
      } else if ( BraceList.Contains(tc.Char()) ) {
        pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
        if ( tc.Char() == '{' )
          istring.NestingLevel++;
        tc.Next();
        return true;
      } else {
        tc.Next();
      }
      return false;
    }

    private CharPos ParseClosingBrace(ITextChars tc, InterpolatedString istring) {
      int position = tc.AbsolutePosition;
      // raw strings close the interpolation with one brace per '$'
      int closingBraces = istring.NestingLevel == 1 && istring.IsRaw ? istring.Dollars : 1;
      int length = 0;
      while ( length < closingBraces && tc.Char() == '}' ) {
        tc.Next();
        length++;
      }
      istring.NestingLevel--;
      if ( istring.NestingLevel == 0 ) {
        // reached the end
        istring.ParsingExpression = false;
      }
      return new CharPos('}', position, EncodedState(), length);
    }

    // parsing the string part
    // if it's an at-string, don't look for escape sequences
    private bool ParseInterpolatedStringText(ITextChars tc, InterpolatedString istring, ref CharPos pos) {
      if ( tc.Char() == '\\' && !istring.Verbatim ) {
        // skip over escape sequences
        tc.Skip(2);
      } else if ( tc.Char() == '{' && tc.NChar() == '{' ) {
        tc.Skip(2);
      } else if ( tc.Char() == '{' ) {
        istring.ParsingExpression = true;
        istring.NestingLevel++;
        pos = new CharPos(tc.Char(), tc.AbsolutePosition, EncodedState());
        tc.Next();
        return true;
      } else if ( istring.Verbatim && tc.Char() == '"' && tc.NChar() == '"' ) {
        // single embedded double quote
        tc.Skip(2);
      } else if ( tc.Char() == '"' ) {
        // done parsing the interpolated string
        tc.Next();
        EndInterpolatedString();
      } else {
        tc.Next();
      }
      return false;
    }

    // C# 11 interpolated raw string: there are no escapes,
    // and an interpolation starts with one brace per '$'.
    // Longer brace runs are content followed by the interpolation.
    private bool ParseInterpolatedRawStringText(ITextChars tc, InterpolatedString istring, ref CharPos pos) {
      if ( tc.Char() == '"' ) {
        if ( SkipQuotes(tc) >= istring.RawQuotes ) {
          EndInterpolatedString();
        }
      } else if ( tc.Char() == '{' ) {
        if ( SkipAll(tc, '{') >= istring.Dollars ) {
          istring.ParsingExpression = true;
          istring.NestingLevel++;
          pos = new CharPos('{', tc.AbsolutePosition - istring.Dollars, EncodedState(), istring.Dollars);
          return true;
        }
      } else {
        tc.Next();
      }
      return false;
    }

    private void EndInterpolatedString() {
      this.istrings.Pop();
      if ( this.istrings.Count == 0 ) {
        this.status = stText;
      }
    }

    private int EncodedState() {
      int encoded = this.status;
      if ( this.istrings.Count > 0 ) {
        var istring = this.istrings.Peek();
        encoded |= Math.Min(istring.NestingLevel, 0xFF) << 8;
        encoded |= Math.Min(istring.RawQuotes, MaxEncodedCount) << RawQuotesShift;
        encoded |= Math.Min(istring.Dollars, MaxEncodedCount) << DollarsShift;
        if ( istring.Verbatim )
          encoded |= VerbatimFlag;
        if ( istring.ParsingExpression )
          encoded |= ParsingExpressionFlag;
      }
      return encoded;
    }

    private sealed class InterpolatedString {
      public bool Verbatim { get; set; }
      public int Dollars { get; set; }
      // number of quotes delimiting a raw string, 0 otherwise
      public int RawQuotes { get; set; }
      public int NestingLevel { get; set; }
      public bool ParsingExpression { get; set; }
      public bool IsRaw => RawQuotes > 0;
    }
  }
}
