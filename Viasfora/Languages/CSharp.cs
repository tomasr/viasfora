using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class CSharp : CBasedLanguage {
    public const String ContentType = "CSharp";
    static readonly String[] CS_KEYWORDS = {
         "if", "else", "while", "do", "for", "foreach", 
         "switch", "break", "continue", "return", "goto", "throw",
         "yield"
      };
    static readonly String[] CS_LINQ_KEYWORDS = {
         "select", "let", "where", "join", "orderby", "group",
         "by", "on", "equals", "into", "from", "descending",
         "ascending"
      };
    static readonly String[] CS_VIS_KEYWORDS = {
         "public", "private", "protected", "internal"
      };
    protected override String[] ControlFlowDefaults {
      get { return CS_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return CS_LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return CS_VIS_KEYWORDS; }
    }
    protected override String KeyName {
      get { return "CSharp"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
  }
}
