using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  public class DefaultLanguage : LanguageInfo {
    static readonly String[] empty = new String[0];
    protected override String[] ControlFlowDefaults {
      get { return empty; }
    }
    protected override String[] LinqDefaults {
      get { return empty; }
    }
    protected override String[] VisibilityDefaults {
      get { return empty; }
    }

    public override string BraceList {
      get { return ""; }
    }
    protected override string[] ContentTypes {
      get { return empty; }
    }
    protected override string KeyName {
      get { return "Text"; }
    }
    public override bool SupportsEscapeSeqs {
      get { return false; }
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new NoBraceExtractor();
    }

    private class NoBraceExtractor : IBraceExtractor {
      private static CharPos[] noBraces = new CharPos[0];
      public IEnumerable<CharPos> Extract(ITextChars text) {
        return noBraces;
      }
    }
  }
}
