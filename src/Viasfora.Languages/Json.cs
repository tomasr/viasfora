using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class JSON : CBasedLanguage, ILanguage {
    public const String ContentType = "JSON";

    protected override String[] SupportedContentTypes
      => new String[] { ContentType };
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public JSON(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new JsonSettings(store, converter);
    }

    protected override IBraceScanner NewBraceScanner()
      => new JScriptBraceScanner();
  }

  public class JsonSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public JsonSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.Json, store, converter) {
    }
  }
}
