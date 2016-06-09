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
    static readonly String[] LINQ_KEYWORDS = {
          "from", "in"
      };
    protected override String[] ControlFlowDefaults => KEYWORDS;
    protected override String[] LinqDefaults => LINQ_KEYWORDS;
    protected override String[] VisibilityDefaults => EMPTY;

    public override String KeyName => Constants.Python;
    protected override String[] SupportedContentTypes
      => new String[] { ContentType };

    protected override IBraceScanner NewBraceScanner() 
      => new PythonBraceScanner();

    [ImportingConstructor]
    public Python(IVsfSettings settings) : base(settings) {
    }
  }
}
