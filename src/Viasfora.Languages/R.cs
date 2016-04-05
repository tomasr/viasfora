using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class R : LanguageInfo {
    static readonly String[] KEYWORDS = {
         "if", "else", "for", "while", "repeat", "switch",
         "return", "next", "break"
      };
    static readonly String[] LINQ_KEYWORDS = {
         "apply", "in"
      };
    public override string KeyName {
      get { return Constants.R; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentTypes.R }; }
    }
    protected override String[] ControlFlowDefaults {
      get { return KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return EMPTY; }
    }


    [ImportingConstructor]
    public R(IVsfSettings settings) : base(settings) {
    }

    protected override IBraceScanner NewBraceScanner() {
      return new RBraceScanner();
    }
    public override IStringScanner NewStringScanner(String text) {
      return new RStringScanner(text);
    }
  }
}
