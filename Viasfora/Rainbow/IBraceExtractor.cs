using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Rainbow {
  public interface IBraceExtractor {
    String BraceList { get; }
    void Reset(int state);
    bool Extract(ITextChars text, ref CharPos pos);
  }
}
