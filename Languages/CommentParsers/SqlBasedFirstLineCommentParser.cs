using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.CommentParsers {
  public class SqlFirstLineCommentParser : IFirstLineCommentParser {
    public string Parse(ITextChars tc) {
      while ( !tc.EndOfLine && Char.IsWhiteSpace(tc.Char()) ) {
        tc.Next();
      }
      if ( tc.EndOfLine ) return null;

      if ( tc.Char() == '/' && tc.NChar() == '*' ) {
        tc.Skip(2);
        // multiline comment
        StringBuilder sb = new StringBuilder();
        while ( !tc.EndOfLine && tc.Char() != '*' && tc.NChar() != '/' ) {
          sb.Append(tc.Char());
          tc.Next();
        }
        return sb.ToString(); 
      } else if ( tc.Char() == '-' && tc.NChar() == '-' ) {
        tc.Skip(2);
        // single line comment
        StringBuilder sb = new StringBuilder();
        while ( !tc.EndOfLine ) {
          sb.Append(tc.Char());
          tc.Next();
        }
        return sb.ToString(); 
      }
      return null;
    }
  }
}
