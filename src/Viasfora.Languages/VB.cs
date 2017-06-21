using Microsoft.VisualStudio.Text.Classification;
using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class VB : LanguageInfo, ILanguage {
    public const String ContentType = "Basic";
    public const String VBScriptContentType = "vbscript";

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, VBScriptContentType }; }
    }
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public VB(ITypedSettingsStore store) {
      this.Settings = new VBSettings(store);
    }

    protected override IBraceScanner NewBraceScanner()
      => new VbBraceScanner();

    public override bool IsKeywordClassification(IClassificationType classificationType) {
      return CompareClassification(classificationType, "Keyword")
          || CompareClassification(classificationType, "VBScript Keyword");
    }
  }

  class VBSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "goto", "resume", "throw", "exit", "stop",
       "do", "loop", "for", "next", "for each",
       "with", "choose", "if", "then", "else", "select",
       "case", "switch", "call", "return", "while"
      };
    protected override String[] LinqDefaults => new String[] {
       "aggregate", "distinct", "equals", "from", "in",
       "group", "join", "let", "order", "by",
       "skip", "take", "where"
      };
    protected override String[] VisibilityDefaults => new String[] {
       "friend", "public", "private", "protected"
      };

    public VBSettings(ITypedSettingsStore store)
      : base (Constants.VB, store) {
    }
  }
}
