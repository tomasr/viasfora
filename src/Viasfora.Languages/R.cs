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

    public override string KeyName => Constants.R;
    protected override String[] SupportedContentTypes
      => new String[] { ContentTypes.R };
    protected override String[] ControlFlowDefaults => KEYWORDS;
    protected override String[] LinqDefaults => LINQ_KEYWORDS;
    protected override String[] VisibilityDefaults => EMPTY;


    [ImportingConstructor]
    public R(IVsfSettings settings) : base(settings) {
    }

    protected override IBraceScanner NewBraceScanner()
      => new RBraceScanner();

    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new RStringScanner(text);
  }
}
