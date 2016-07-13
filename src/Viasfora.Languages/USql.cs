using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class USql : LanguageInfo {

    static readonly String[] QUERY = {
      "select", "extract", "process", "reduce", "combine",
      "produce", "using", "output"
    };
    static readonly String[] VISIBILITY = {
      "readonly"
    };

    public override String KeyName {
      get { return "U-SQL"; }
    }

    protected override String[] ControlFlowDefaults {
      get { return EMPTY; }
    }

    protected override String[] LinqDefaults {
      get { return QUERY; }
    }

    protected override string[] VisibilityDefaults {
      get { return VISIBILITY; }
    }

    protected override String[] SupportedContentTypes {
      get { return new String[] { "U-SQL" }; }
    }

    protected override IBraceScanner NewBraceScanner() {
      return new USqlBraceScanner();
    }

    [ImportingConstructor]
    public USql(IVsfSettings settings) : base(settings) {
    }
  }
}
