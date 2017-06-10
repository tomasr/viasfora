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
  public class R : LanguageInfo, ILanguage {
    protected override String[] SupportedContentTypes
      => new String[] { ContentTypes.R };
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public R(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new RSettings(store, converter);
    }

    protected override IBraceScanner NewBraceScanner()
      => new RBraceScanner();

    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new RStringScanner(text);
  }

  class RSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "if", "else", "for", "while", "repeat", "switch",
       "return", "next", "break"
      };
    protected override String[] LinqDefaults => new String[] {
       "apply", "in"
      };
    protected override String[] VisibilityDefaults => EMPTY;

    public RSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.R, store, converter) {
    }
  }
}
