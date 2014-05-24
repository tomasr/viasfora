using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.BraceExtractors {
  public class CssBraceExtractor : IBraceExtractor {
    const int stText = 0;
    const int stComment = 1;
    const int stSingleQuotedString = 2;
    const int stDoubleQuotedString = 3;
    private int state;
    private LanguageInfo lang;

    public CssBraceExtractor(LanguageInfo lang) {
      this.lang = lang;
    }

    public void Reset() {
      this.state = stText;
    }

    public IEnumerable<CharPos> Extract(ITextChars text) {
      throw new NotImplementedException();
    }
  }
}
