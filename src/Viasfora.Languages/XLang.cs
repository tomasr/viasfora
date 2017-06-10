using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class XLang : CBasedLanguage {
    public const String ContentType = ContentTypes.XLang;

    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public override String KeyName => Constants.XLang;

    protected override String[] SupportedContentTypes
      => new String[] { ContentType };

    protected override IBraceScanner NewBraceScanner()
      => new CSharpBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new CSharpStringScanner(text);

    [ImportingConstructor]
    public XLang(IVsfSettings settings) : base(settings) {
    }
  }
}
