using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class Python : LanguageInfo, ILanguage {
    public static readonly String[] knownContentTypes = new String[] {
        "Python", "code++.Python"
    };
    public ILanguageSettings Settings { get; private set; }

    protected override String[] SupportedContentTypes
      => knownContentTypes;

    protected override IBraceScanner NewBraceScanner() 
      => new PythonBraceScanner();

    [ImportingConstructor]
    public Python(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new PythonSettings(store, converter);
    }
  }

  class PythonSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
          "break", "continue", "if", "elif", "else",
          "for", "raise", "return", "while", "yield"
      };
    protected override String[] LinqDefaults => new String[] {
          "from", "in"
      };
    protected override String[] VisibilityDefaults => EMPTY;

    public PythonSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.Python, store, converter) {
    }
  }
}
