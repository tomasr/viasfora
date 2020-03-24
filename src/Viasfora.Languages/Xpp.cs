using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class Xpp : CBasedLanguage, ILanguage {
    public const String ContentType = "X++";

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType }; }
    }

    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public Xpp(ITypedSettingsStore store) {
      this.Settings = new XppSettings(store);
    }

    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new CStringScanner(text);
  }

  class XppSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "if", "else", "while", "do", "for", "switch",
       "break", "continue", "return", "throw", "pause", "next"
      };

    protected override String[] LinqDefaults => new string[] {
      "select", "where", "sum", "outer", "order", "like",
      "join", "group", "from", "exists", "count", "avg"
    };

    protected override String[] VisibilityDefaults => new String[] {
      "public", "private", "protected"
    };

    public XppSettings(ITypedSettingsStore store)
      : base(Langs.Xpp, store) {
    }
  }
}
