using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public class CBasedFirstLineCommentParser : IFirstLineCommentParser {
    public string Parse(ITextChars tc) {
      while ( !tc.EndOfLine && tc.Char() != '/' ) {
        tc.Next();
      }
      if ( tc.EndOfLine ) return null;

      StringBuilder sb = new StringBuilder();
      if ( tc.NChar() == '*' ) {
        // multiline comment
        while ( !tc.EndOfLine && tc.NChar() != '*' && tc.NNChar() != '/' ) {
          sb.Append(tc.Char());
          tc.Next();
        }
      } else if ( tc.NChar() == '/' ) {
        // single line comment
        while ( !tc.EndOfLine ) {
          sb.Append(tc.Char());
          tc.Next();
        }
      }
      return sb.ToString(); 
    }
  }
}
