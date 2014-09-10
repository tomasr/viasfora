using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  public abstract class CBasedLanguage : LanguageInfo {
    public override string BraceList {
      get { return "(){}[]"; }
    }

    public override IBraceExtractor NewBraceExtractor() {
      return new CBraceExtractor(this.BraceList);
    }
    public override IEscapeSequenceParser NewEscapeSequenceParser(String text) {
      return new CEscapeSequenceParser(text);
    }
  }
}
