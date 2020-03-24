using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class CSharp : CBasedLanguage, ILanguage {
    public const String ContentType = "CSharp";
    protected override String[] SupportedContentTypes
      => new String[] { ContentType };
    public ILanguageSettings Settings { get; private set; }

    protected override IBraceScanner NewBraceScanner()
      => new CSharpBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new CSharpStringScanner(text, classificationName);

    public override bool IsKeywordClassification(String classificationType) {
      return base.IsKeywordClassification(classificationType)
          || CompareClassification(classificationType, "keyword - control");

    }
    [ImportingConstructor]
    public CSharp(ITypedSettingsStore store) {
      this.Settings = new CSharpSettings(store);
    }

  }

  public class CSharpSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
         "if", "else", "while", "do", "for", "foreach",
         "switch", "break", "continue", "return", "goto", "throw",
         "yield"
      };
    protected override String[] LinqDefaults => new String[] {
         "select", "let", "where", "join", "orderby", "group",
         "by", "on", "equals", "into", "from", "descending",
         "ascending"
      };
    protected override String[] VisibilityDefaults => new String[] {
         "public", "private", "protected", "internal"
      };

    public CSharpSettings(ITypedSettingsStore store)
      : base (Langs.CSharp, store) {
    }
  }
}
