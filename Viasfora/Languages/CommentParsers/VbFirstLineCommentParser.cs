using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.CommentParsers {
  public class VbFirstLineCommentParser : IFirstLineCommentParser {
    public string Parse(ITextChars tc) {
      while ( !tc.EndOfLine && tc.Char() != '\'' ) {
        tc.Next();
      }
      if ( tc.EndOfLine ) return null;

      // single line comment
      return tc.GetRemainder();
    }
  }
}
