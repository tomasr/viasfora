using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public interface IFirstLineCommentParser {
    String Parse(ITextChars tc);
  }
}
