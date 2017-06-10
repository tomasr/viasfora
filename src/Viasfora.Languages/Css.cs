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
  public class Css : LanguageInfo, ILanguage {
    public const String ContentType = "css";
    public const String SassContentType = "SCSS";
    public const String LessContentType = "LESS";

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, SassContentType, LessContentType }; }
    }
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public Css(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new CssSettings(store, converter);
    }

    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new CssStringScanner(text);
    protected override IBraceScanner NewBraceScanner()
      => new CssBraceScanner();
  }

  public class CssSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public CssSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.Css, store, converter) {
    }
  }
}
