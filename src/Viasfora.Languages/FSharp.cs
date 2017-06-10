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
  public class FSharp : LanguageInfo, ILanguage {
    public const String ContentType = "F#";
    protected override String[] SupportedContentTypes
      => new String[] { ContentType };
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public FSharp(ISettingsStore store, IStorageConversions converter) {
      this.Settings = new FSharpSettings(store, converter);
    }

    protected override IBraceScanner NewBraceScanner()
      => new FSharpBraceScanner();
    public override IStringScanner NewStringScanner(String classificationName, String text)
      => new FSharpStringScanner(text);
  }

  class FSharpSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "if", "then", "elif", "else", "match", "with",
       "for", "do", "to", "done", "while", "rec",
         "failwith", "yield"
      };
    protected override String[] LinqDefaults => new String[] {
       "query", "select", "seq"
      };
    protected override String[] VisibilityDefaults => new String[] {
       "public", "private", "internal"
      };

    public FSharpSettings(ISettingsStore store, IStorageConversions converter)
      : base (Constants.FSharp, store, converter) {
    }
  }
}
