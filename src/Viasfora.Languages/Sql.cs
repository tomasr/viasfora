using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
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

    protected override String[] ControlFlowDefaults => KEYWORDS;
    protected override String[] LinqDefaults => LINQ_KEYWORDS;
    protected override String[] VisibilityDefaults => VIS_KEYWORDS;
    public override String KeyName => Constants.Sql;
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, ContentTypeAlt }; }
    }

    protected override IBraceScanner NewBraceScanner()
      => new SqlBraceScanner();

    [ImportingConstructor]
    public Sql(IVsfSettings settings) : base(settings) {
    }

    protected override string TextToCompare(string text) {
      // the SQL classifier will return text spans that include
      // trailing spaces (such as "IF ")
      return text.Trim();
    }
  }
}
