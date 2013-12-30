using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  class Sql : LanguageInfo {
    public const String ContentType = "Sql Server Tools";
    public const String ContentTypeAlt = "SQL";

    static readonly String[] KEYWORDS = {
          "begin", "end", "break", "continue", "goto", "if",
          "else", "then", "return", "throw", "try", "catch",
          "waitfor", "while"
      };
    static readonly String[] VIS_KEYWORDS = {
         "public", "external"
      };
    static readonly String[] LINQ_KEYWORDS = {
         "select", "update", "insert", "delete", "merge"
      };
    public override bool SupportsEscapeSeqs {
      get { return false; }
    }
    public override string BraceList {
      get { return "()"; }
    }
    protected override String[] ControlFlowDefaults {
      get { return KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return VIS_KEYWORDS; }
    }
    protected override String KeyName {
      get { return "Sql"; }
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new SqlBraceExtractor(this);
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType, ContentTypeAlt }; }
    }
    protected override string TextToCompare(string text) {
      // the SQL classifier will return text spans that include
      // trailing spaces (such as "IF ")
      return text.Trim();
    }
  }
}
