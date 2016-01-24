using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  public abstract class CBasedLanguage : LanguageInfo {

    public CBasedLanguage(IVsfSettings settings) : base(settings) {
    }
    public override IBraceScanner NewBraceScanner() {
      return new CBraceScanner();
    }
    public override IStringScanner NewStringScanner(String text) {
      return new BasicCStringScanner(text);
    }
  }
}
