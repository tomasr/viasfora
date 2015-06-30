using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceExtractors {
  public class DefaultBraceExtractor : IBraceExtractor {
    public string BraceList {
      get { return String.Empty; }
    }

    public int ReparseWindow {
      get { return 0; }
    }

    public void Reset() {
    }

    public IEnumerable<CharPos> Extract(ITextChars text) {
      return Enumerable.Empty<CharPos>();
    }
  }
}
