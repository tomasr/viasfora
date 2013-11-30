using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  class VB : LanguageInfo {
    public const String ContentType = "Basic";
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
      get { return new String[] { ContentType }; }
    }
    public override string BraceList {
      get { return "()"; }
    }

    public override bool IsMultiLineCommentStart(string text, int pos) {
      return false;
    }
    public override bool IsMultiLineCommentEnd(string text, int pos) {
      return false;
    }
    public override bool IsSingleLineCommentStart(string text, int pos) {
      return text[pos] == '\'';
    }
    public override bool IsMultiLineStringStart(string text, int pos, out char quote) {
      quote = '\0';
      return false;
    }
    public override bool IsSingleLineStringStart(string text, int pos, out char quote) {
      quote = '"';
      return text[pos] == '"';
    }
    public override bool IsStringEnd(string text, int pos, char quote) {
      return text[pos] == quote;
    }
  }
}
