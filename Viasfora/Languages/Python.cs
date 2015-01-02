using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

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
    public override IBraceExtractor NewBraceExtractor() {
      return new PythonBraceExtractor();
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
  }
}
