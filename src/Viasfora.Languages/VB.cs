using Microsoft.VisualStudio.Text.Classification;
using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class VB : LanguageInfo {
    public const String ContentType = "Basic";
    public const String VBScriptContentType = "vbscript";

    static readonly String[] VB_KEYWORDS = {
         "goto", "resume", "throw", "exit", "stop",
         "do", "loop", "for", "next", "for each",
         "with", "choose", "if", "then", "else", "select",
         "case", "switch", "call", "return", "while"
      };
    static readonly String[] VB_VIS_KEYWORDS = {
         "friend", "public", "private", "protected"
      };
    static readonly String[] VB_LINQ_KEYWORDS = {
         "aggregate", "distinct", "equals", "from", "in",
         "group", "join", "let", "order", "by",
         "skip", "take", "where"
      };
    protected override String[] ControlFlowDefaults => VB_KEYWORDS;
    protected override String[] LinqDefaults => VB_LINQ_KEYWORDS;
    protected override String[] VisibilityDefaults => VB_VIS_KEYWORDS;
    public override String KeyName => Constants.VB;

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, VBScriptContentType }; }
    }

    [ImportingConstructor]
    public VB(IVsfSettings settings) : base(settings) {
    }

    protected override IBraceScanner NewBraceScanner()
      => new VbBraceScanner();

    public override bool IsKeywordClassification(IClassificationType classificationType) {
      return CompareClassification(classificationType, "Keyword")
          || CompareClassification(classificationType, "VBScript Keyword");
    }
  }
}
