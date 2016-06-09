using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Languages {
  public class DefaultLanguage : LanguageInfo {
    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    protected override string[] SupportedContentTypes => EMPTY;
    public override string KeyName => "Text";

    [ImportingConstructor]
    public DefaultLanguage(IVsfSettings settings) : base(settings) {
    }

    public override bool MatchesContentType(IContentType contentType) {
      return true;
    }
    protected override IBraceScanner NewBraceScanner()
      => new DefaultBraceScanner();
  }
}
