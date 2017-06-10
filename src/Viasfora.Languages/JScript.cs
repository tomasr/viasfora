using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class JScript : CBasedLanguage, ILanguage {
    private readonly static String[] knownTypes =
      new String[] { "JScript", "JavaScript", "Node.js" };

    protected override String[] SupportedContentTypes => knownTypes;
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public JScript(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new JScriptSettings(store, converter);
    }

    protected override IBraceScanner NewBraceScanner()
      => new JScriptBraceScanner();
  }

  public class JScriptSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
         "if", "else", "while", "do", "for", "switch",
         "break", "continue", "return", "throw"
      };
    protected override String[] LinqDefaults => new String[] {
         "in", "with"
      };
    protected override String[] VisibilityDefaults => EMPTY;

    public JScriptSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.JS, store, converter) {
    }
  }
}
