using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  class JScript : CBasedLanguage {
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
    protected override String KeyName {
      get { return "JScript"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType, ContentTypeVS2012 }; }
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new JScriptBraceExtractor(this);
    }
  }
}
