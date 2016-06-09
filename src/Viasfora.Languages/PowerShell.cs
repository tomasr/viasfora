using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class PowerShell : LanguageInfo {
    public const String ContentTypeVS2013 = "PowerShell.v3";
    public const String ContentTypePSTools = "PowerShell";
    static readonly String[] FLOW_KEYWORDS = {
          "for", "while", "foreach", "if", "else",
          "elseif", "do", "break", "continue",
          "exit", "return", "until", "switch"
      };
    protected override String[] ControlFlowDefaults => FLOW_KEYWORDS;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    public override String KeyName => Constants.PowerShell; 

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentTypePSTools, ContentTypeVS2013 }; }
    }

    [ImportingConstructor]
    public PowerShell(IVsfSettings settings) : base(settings) {
    }
    protected override IBraceScanner NewBraceScanner()
      => new PsBraceScanner();
    public override IStringScanner NewStringScanner(String text)
      => new PsStringScanner(text);
  }
}
