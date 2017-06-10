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
  public class XLang : CBasedLanguage, ILanguage {
    public const String ContentType = ContentTypes.XLang;

    protected override String[] SupportedContentTypes
      => new String[] { ContentType };
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public XLang(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new XLangSettings(store, converter);
    }

    protected override IBraceScanner NewBraceScanner()
      => new CSharpBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new CSharpStringScanner(text);
  }

  class XLangSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public XLangSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.XLang, store, converter) {
    }
  }
}
