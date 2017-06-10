using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  public abstract class CBasedLanguage : LanguageInfo {

    public CBasedLanguage(IVsfSettings settings) : base(settings) {
    }
    protected override IBraceScanner NewBraceScanner()
      => new CBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new BasicCStringScanner(text);
  }
}
