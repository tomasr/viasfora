using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.CommentParsers;
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
    protected override String[] ControlFlowDefaults {
      get { return VB_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return VB_LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return VB_VIS_KEYWORDS; }
    }
    public override String KeyName {
      get { return Constants.VB; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, VBScriptContentType }; }
    }

    [ImportingConstructor]
    public VB(IVsfSettings settings) : base(settings) {
    }

    public override IBraceExtractor NewBraceExtractor() {
      return new VbBraceExtractor();
    }
  }
}
