using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class Cpp : CBasedLanguage, ILanguage {
    private readonly static String[] knownTypes =
      new String[] { "C/C++", "HLSL" };

    protected override String[] SupportedContentTypes
      => knownTypes;
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public Cpp(ITypedSettingsStore store) {
      this.Settings = new CppSettings(store);
    }

    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new CStringScanner(text);
  }

  class CppSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "if", "else", "while", "do", "for", "each", "switch",
       "break", "continue", "return", "goto", "throw"
      };

    protected override String[] LinqDefaults => EMPTY;

    protected override String[] VisibilityDefaults => new String[] {
      "public", "private", "protected", "internal", "friend"
    };

    public CppSettings(ITypedSettingsStore store)
      : base(Constants.Cpp, store) {
    }
  }
}
