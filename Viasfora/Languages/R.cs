using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
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
         "return"
      };
    static readonly String[] LINQ_KEYWORDS = {
         "apply"
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

    public override IBraceScanner NewBraceScanner() {
      return new RBraceScanner();
    }
    public override IStringScanner NewStringScanner(String text) {
      return new RStringScanner(text);
    }
  }
}
