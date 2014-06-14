using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
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
    protected override String KeyName {
      get { return "VB"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType, VBScriptContentType }; }
    }
    public override string BraceList {
      get { return "()"; }
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new VbBraceExtractor(this);
    }
  }
}
