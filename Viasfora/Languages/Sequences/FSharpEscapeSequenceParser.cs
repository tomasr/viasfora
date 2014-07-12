using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class FSharpEscapeSequenceParser : IEscapeSequenceParser {
    private ITextChars text;
    private const String escapeChar = "\"\\'ntbrafv";

    public FSharpEscapeSequenceParser(String text) {
      this.text = new StringChars(text);
      // always skip the first char
      // (since quotes are included in the string)
      this.text.Next();
      // If this is an at-string, or a C preprocessor include
      // skip it
      if ( text.StartsWith("@") || text.StartsWith("<") ) {
        this.text.SkipRemainder();
      }

      if ( text.StartsWith("\"\"\"") ) {
        this.text.SkipRemainder();
      }
    }
    public Span? Next() {
      while ( !text.EndOfLine ) {
        if ( text.Char() == '\\' ) {
          text.Next();
          if ( escapeChar.IndexOf(text.Char()) >= 0 ) {
            text.Next();
            return new Span(text.Position - 2, 2);
          }
          if ( Char.IsDigit(text.Char()) && Char.IsDigit(text.NChar()) && Char.IsDigit(text.NNChar()) ) {
            // a trigraph
            text.Skip(3);
            return new Span(text.Position-4, 4);
          }
          if ( text.Char() == '0' && !Char.IsDigit(text.NChar()) ) {
            // \0
            text.Next();
            return new Span(text.Position-2, 2);
          }
          if ( text.Char() == 'u' ) {
            text.Next();
            text.Mark();
            Span? span = TryParseShortUnicode();
            if ( span.HasValue ) {
              return span.Value;
            }
            text.BackToMark();
          }
          if ( text.Char() == 'U' ) {
            text.Next();
            text.Mark();
            Span? span = TryParseLongUnicode();
            if ( span.HasValue ) {
              return span.Value;
            }
            text.BackToMark();
          }
        } else {
          text.Next();
        }
      }
      return null;
    }

    private Span? TryParseShortUnicode() {
      // \Uhhhhhhhh
      for ( int i = 0; i < 4; i++ ) {
        if ( !text.Char().IsHexDigit() ) {
          return null;
        }
        text.Next();
      }
      return new Span(text.Position - 6, 6);
    }
    private Span? TryParseLongUnicode() {
      // \Uhhhhhhhh
      for ( int i = 0; i < 8; i++ ) {
        if ( !text.Char().IsHexDigit() ) {
          return null;
        }
        text.Next();
      }
      return new Span(text.Position - 10, 10);
    }
  }
}
