using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class JScript : CBasedLanguage {
    private readonly static String[] knownTypes =
      new String[] { "JScript", "JavaScript", "Node.js" };

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
    public override String KeyName {
      get { return Constants.JS; }
    }
    protected override String[] SupportedContentTypes {
      get { return knownTypes; }
    }

    [ImportingConstructor]
    public JScript(IVsfSettings settings) : base(settings) {
    }

    public override IBraceScanner NewBraceScanner() {
      return new JScriptBraceScanner();
    }
  }
}
