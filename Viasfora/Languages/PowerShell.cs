using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class PowerShell : LanguageInfo {
    public const String ContentType = "PowerShell.v3";
    static readonly String[] FLOW_KEYWORDS = {
          "for", "while", "foreach", "if", "else",
          "elseif", "do", "break", "continue",
          "exit", "return", "until", "switch"
      };
    static readonly String[] QUERY_KEYWORDS = {
      };
    static readonly String[] VIS_KEYWORDS = {
      };
    public override string BraceList {
      get { return "(){}[]"; }
    }

    public override IBraceExtractor NewBraceExtractor() {
      return new PsBraceExtractor(this);
    }
    public override IEscapeSequenceParser NewEscapeSequenceParser(String text) {
      return new PsEscapeSequenceParser(text);
    }
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
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
  }
}
