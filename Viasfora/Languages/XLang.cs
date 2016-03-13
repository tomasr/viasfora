using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class XLang : CBasedLanguage {
    public const String ContentType = ContentTypes.XLang;
    protected override String[] ControlFlowDefaults {
      get { return EMPTY; }
    }
    protected override String[] LinqDefaults {
      get { return EMPTY; }
    }
    protected override String[] VisibilityDefaults {
      get { return EMPTY; }
    }
    public override String KeyName {
      get { return Constants.XLang; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType }; }
    }

    [ImportingConstructor]
    public XLang(IVsfSettings settings) : base(settings) {
    }
  }
}
