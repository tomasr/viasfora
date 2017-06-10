using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class FSharp : LanguageInfo {
    public const String ContentType = "F#";
    static readonly String[] KEYWORDS = {
         "if", "then", "elif", "else", "match", "with",
         "for", "do", "to", "done", "while", "rec",
         "failwith", "yield"
      };
    static readonly String[] LINQ_KEYWORDS = {
         "query", "select", "seq"
      };
    static readonly String[] VIS_KEYWORDS = {
         "public", "private", "internal"
      };
    protected override String[] ControlFlowDefaults => KEYWORDS;
    protected override String[] LinqDefaults => LINQ_KEYWORDS;
    protected override String[] VisibilityDefaults => VIS_KEYWORDS;
    public override String KeyName => Constants.FSharp;
    protected override String[] SupportedContentTypes
      => new String[] { ContentType };

    [ImportingConstructor]
    public FSharp(IVsfSettings settings) : base(settings) {
    }

    protected override IBraceScanner NewBraceScanner()
      => new FSharpBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new FSharpStringScanner(text);
  }
}
