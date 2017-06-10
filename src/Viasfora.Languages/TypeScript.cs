using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class TypeScript : CBasedLanguage, ILanguage {
    public const String ContentType = "TypeScript";

    protected override String[] SupportedContentTypes
      => new String[] { ContentType };
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public TypeScript(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new TypeScriptSettings(store, converter);
    }

    protected override IBraceScanner NewBraceScanner()
      => new JScriptBraceScanner();
  }

  class TypeScriptSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "if", "else", "while", "do", "for", "switch",
       "break", "continue", "return", "throw"
      };
    protected override String[] LinqDefaults => new String[] {
       "in", "with"
      };
    protected override String[] VisibilityDefaults => new String[] {
       "export", "public", "private"
      };

    public TypeScriptSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.TypeScript, store, converter) {
    }
  }
}
