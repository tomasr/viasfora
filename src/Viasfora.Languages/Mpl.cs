using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class Mpl : LanguageInfo, ILanguageWithStrings {
    public const String ContentType = "MPL";

    protected override String[] SupportedContentTypes
      => new String[] { ContentType };

    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public Mpl(ITypedSettingsStore store) {
      this.Settings = new MplSettings(store);
    }

    protected override IBraceScanner NewBraceScanner()
      => new MplBraceScanner();

    public override IStringScanner NewStringScanner(String classificationName, String text) {
      return new CStringScanner(text);
    }

    public override bool IsKeywordClassification(String classificationType) {
      var comp = StringComparer.OrdinalIgnoreCase;
      return comp.Equals(classificationType, "MplBuiltin") || comp.Equals(classificationType, "MplLabel");
    }
    public bool IsStringClassification(String classificationType) {
      var comp = StringComparer.OrdinalIgnoreCase;
      return comp.Equals(classificationType, "MplText");
    }
  }

  class MplSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
      "times", "when", "while", "call", "if", "loop"
    };
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public MplSettings(ITypedSettingsStore store)
      : base(Langs.Mpl, store) {
    }
  }
}
