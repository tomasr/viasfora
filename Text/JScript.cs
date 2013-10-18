using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  class JScript : LanguageKeywords {
    public const String ContentType = "JScript";
    public const String ContentTypeVS2012 = "JavaScript";

    static readonly String[] JS_KEYWORDS = {
         "if", "else", "while", "do", "for", "switch",
         "break", "continue", "return", "throw"
      };
    static readonly String[] JS_LINQ_KEYWORDS = {
         "in", "with"
      };
    protected override String[] ControlFlowDefaults {
      get { return JS_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return JS_LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return new String[0]; }
    }
    protected override string KeyName {
      get { return "JScript"; }
    }
  }
}
