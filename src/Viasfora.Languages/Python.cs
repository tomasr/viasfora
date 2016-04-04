using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class Python : LanguageInfo {
    public const String ContentType = "Python";

    static readonly String[] KEYWORDS = {
          "break", "continue", "if", "elif", "else",
          "for", "raise", "return", "while", "yield"
      };
    static readonly String[] VIS_KEYWORDS = {
      };
    static readonly String[] LINQ_KEYWORDS = {
          "from", "in"
      };
    protected override String[] ControlFlowDefaults {
      get { return KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return VIS_KEYWORDS; }
    }
    public override String KeyName {
      get { return Constants.Python; }
    }
    protected override IBraceScanner NewBraceScanner() {
      return new PythonBraceScanner();
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType }; }
    }

    [ImportingConstructor]
    public Python(IVsfSettings settings) : base(settings) {
    }
  }
}
