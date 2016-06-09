using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;

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

    [ImportingConstructor]
    public XLang(IVsfSettings settings) : base(settings) {
    }
  }
}
