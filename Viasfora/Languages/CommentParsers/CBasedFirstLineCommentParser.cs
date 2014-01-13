using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.CommentParsers {
  public class CBasedFirstLineCommentParser : IFirstLineCommentParser {
    public string Parse(ITextChars tc) {
      while ( !tc.EndOfLine && tc.Char() != '/' ) {
        tc.Next();
      }
      if ( tc.EndOfLine ) return null;

      StringBuilder sb = new StringBuilder();
      tc.Next();
      if ( tc.Char() == '*' ) {
        tc.Next();
        // multiline comment
        while ( !tc.EndOfLine && tc.Char() != '*' && tc.NChar() != '/' ) {
          sb.Append(tc.Char());
          tc.Next();
        }
      } else if ( tc.Char() == '/' ) {
        tc.Next();
        // single line comment
        sb.Append(tc.GetRemainder());
      }
      return sb.ToString(); 
    }
  }
}
