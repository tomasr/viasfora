using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  class PowerShell : LanguageInfo {
    public const String ContentType = "PowerShell.v3";
    static readonly String[] FLOW_KEYWORDS = {
          "for", "while", "foreach", "if", "else",
          "do", "break", "continue", "continue",
          "exit"
      };
    static readonly String[] QUERY_KEYWORDS = {
      };
    static readonly String[] VIS_KEYWORDS = {
      };
    public override string BraceList {
      get { return "(){}[]"; }
    }
    public override bool SupportsEscapeSeqs {
      get { return false; }
    }

    public override IBraceExtractor NewBraceExtractor() {
      return new PSBraceExtractor(this);
    }
    protected override String[] ControlFlowDefaults {
      get { return FLOW_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return QUERY_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return QUERY_KEYWORDS; }
    }
    protected override String KeyName {
      get { return "PowerShell"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
  }
}
