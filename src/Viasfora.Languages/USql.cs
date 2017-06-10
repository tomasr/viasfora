using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class USql : LanguageInfo {

    static readonly String[] QUERY = {
      "select", "extract", "process", "reduce", "combine",
      "produce", "using", "output", "from"
    };
    static readonly String[] VISIBILITY = {
      "readonly"
    };

    public override String KeyName {
      get { return Constants.USql; }
    }

    protected override String[] ControlFlowDefaults {
      get { return EMPTY; }
    }

    protected override String[] LinqDefaults {
      get { return QUERY; }
    }

    protected override String[] VisibilityDefaults {
      get { return VISIBILITY; }
    }

    protected override String[] SupportedContentTypes {
      get { return new String[] { "U-SQL" }; }
    }

    protected override IBraceScanner NewBraceScanner() {
      return new USqlBraceScanner();
    }
    public override IStringScanner NewStringScanner(String classificationName, String text) {
      return new CSharpStringScanner(text, classificationName);
    }

    [ImportingConstructor]
    public USql(IVsfSettings settings) : base(settings) {
    }
  }
}
