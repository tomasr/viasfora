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
    static readonly String[] QUERY_KEYWORDS = {
      };
    static readonly String[] VIS_KEYWORDS = {
      };
    protected override String[] ControlFlowDefaults {
      get { return FLOW_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return QUERY_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return VIS_KEYWORDS; }
    }
    public override String KeyName {
      get { return Constants.PowerShell; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentTypePSTools, ContentTypeVS2013 }; }
    }

    [ImportingConstructor]
    public PowerShell(IVsfSettings settings) : base(settings) {
    }
    public override IBraceScanner NewBraceScanner() {
      return new PsBraceScanner();
    }
    public override IStringScanner NewStringScanner(String text) {
      return new PsStringScanner(text);
    }
  }
}
