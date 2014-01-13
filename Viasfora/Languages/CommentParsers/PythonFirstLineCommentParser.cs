using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.CommentParsers {
  public class PythonFirstLineCommentParser : IFirstLineCommentParser {
    public string Parse(ITextChars tc) {
      while ( !tc.EndOfLine && tc.Char() != '#' ) {
        tc.Next();
      }
      if ( tc.EndOfLine ) return null;

      return tc.GetRemainder();
    }
  }
}
