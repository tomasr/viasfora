using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
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
    protected override String KeyName {
      get { return "Cpp"; }
    }
  }
}
