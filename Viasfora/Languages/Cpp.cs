using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class Cpp : CBasedLanguage {
    public const String ContentType = "C/C++";
    static readonly String[] CPP_KEYWORDS = {
         "if", "else", "while", "do", "for", "each", "switch",
         "break", "continue", "return", "goto", "throw"
      };
    static readonly String[] CPP_VIS_KEYWORDS = {
         "public", "private", "protected", "internal", "friend"
      };
    protected override String[] ControlFlowDefaults {
      get { return CPP_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return new String[0]; }
    }
    protected override String[] VisibilityDefaults {
      get { return CPP_VIS_KEYWORDS; }
    }
    public override String KeyName {
      get { return Constants.Cpp; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
  }
}
