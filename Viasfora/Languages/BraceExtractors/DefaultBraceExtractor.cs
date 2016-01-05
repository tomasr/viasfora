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

    public void Reset(int state) {
    }

    public bool Extract(ITextChars text, ref CharPos pos) {
      return false;
    }
  }
}
