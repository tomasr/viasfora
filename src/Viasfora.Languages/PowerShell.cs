using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class PowerShell : LanguageInfo, ILanguage {
    public const String ContentTypeVS2013 = "PowerShell.v3";
    public const String ContentTypePSTools = "PowerShell";

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentTypePSTools, ContentTypeVS2013 }; }
    }
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public PowerShell(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new PowershellSettings(store, converter);
    }
    protected override IBraceScanner NewBraceScanner()
      => new PsBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new PsStringScanner(text);
  }

  public class PowershellSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
        "for", "while", "foreach", "if", "else",
        "elseif", "do", "break", "continue",
        "exit", "return", "until", "switch"
      };
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public PowershellSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.PowerShell, store, converter) {
    }
  }
}
