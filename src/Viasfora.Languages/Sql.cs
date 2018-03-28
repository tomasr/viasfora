using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class Sql : LanguageInfo, ILanguage {
    private readonly static String[] knownContentTypes =
      new String[] { "Sql Server Tools", "SQL", "StreamAnalytics" };
    protected override String[] SupportedContentTypes => knownContentTypes;
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public Sql(ITypedSettingsStore store) {
      this.Settings = new SqlSettings(store);
      // the SQL classifier will return text spans that include
      // trailing spaces (such as "IF ")
      this.NormalizationFunction = text => text.Trim();
    }

    protected override IBraceScanner NewBraceScanner()
      => new SqlBraceScanner();
  }

  class SqlSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
        "begin", "end", "break", "continue", "goto", "if",
        "else", "then", "return", "throw", "try", "catch",
        "waitfor", "while"
      };
    protected override String[] LinqDefaults => new String[] {
       "select", "update", "insert", "delete", "merge"
      };
    protected override String[] VisibilityDefaults => new String[] {
       "public", "external"
      };

    public SqlSettings(ITypedSettingsStore store)
      : base (Langs.Sql, store) {
    }
  }
}
