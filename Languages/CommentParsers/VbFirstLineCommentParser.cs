using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public class VbFirstLineCommentParser : IFirstLineCommentParser {
    public string Parse(ITextChars tc) {
      while ( !tc.EndOfLine && tc.Char() != '\'' ) {
        tc.Next();
      }
      if ( tc.EndOfLine ) return null;

      StringBuilder sb = new StringBuilder();
      // single line comment
      while ( !tc.EndOfLine ) {
        sb.Append(tc.Char());
        tc.Next();
      }
      return sb.ToString(); 
    }
  }
}
