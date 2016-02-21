using System;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceScanners {
  public class DefaultBraceScanner : IBraceScanner {
    public string BraceList {
      get { return String.Empty; }
    }

    public void Reset(int state) {
    }

    public bool Extract(ITextChars text, ref CharPos pos) {
      text.SkipRemainder();
      return false;
    }
  }
}
