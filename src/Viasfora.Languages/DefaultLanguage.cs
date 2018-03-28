using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Languages {
  public class DefaultLanguage : LanguageInfo, ILanguage {

    protected override string[] SupportedContentTypes => new String[0];
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public DefaultLanguage(ITypedSettingsStore store) {
      this.Settings = new DefaultSettings(store);
    }

    public override bool MatchesContentType(Func<String, bool> _) {
      return true;
    }
    protected override IBraceScanner NewBraceScanner()
      => new DefaultBraceScanner();
  }

  public class DefaultSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public DefaultSettings(ITypedSettingsStore store)
      : base ("Text", store) {
    }
  }
}
