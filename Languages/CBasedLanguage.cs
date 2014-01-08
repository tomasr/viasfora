using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  abstract class CBasedLanguage : LanguageInfo {
    public override string BraceList {
      get { return "(){}[]"; }
    }
    public override bool SupportsEscapeSeqs {
      get { return true; }
    }

    public override IBraceExtractor NewBraceExtractor() {
      return new CBraceExtractor(this);
    }
    public override IFirstLineCommentParser NewFirstLineCommentParser() {
      return new CBasedFirstLineCommentParser();
    }
  }
}
