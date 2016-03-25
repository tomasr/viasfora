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
      get { return Constants.FSharp; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType }; }
    }

    [ImportingConstructor]
    public FSharp(IVsfSettings settings) : base(settings) {
    }

    public override IBraceScanner NewBraceScanner() {
      return new FSharpBraceScanner();
    }
    public override IStringScanner NewStringScanner(String text) {
      return new FSharpStringScanner(text);
    }
  }
}
