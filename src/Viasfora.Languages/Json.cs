using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class JSON : CBasedLanguage {
    public const String ContentType = "JSON";

    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public override String KeyName => Constants.Json;
    protected override String[] SupportedContentTypes
      => new String[] { ContentType };

    [ImportingConstructor]
    public JSON(IVsfSettings settings) : base(settings) {
    }

    protected override IBraceScanner NewBraceScanner()
      => new JScriptBraceScanner();
  }
}
