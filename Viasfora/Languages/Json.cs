using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class JSON : CBasedLanguage {
    public const String ContentType = "JSON";

    protected override String[] ControlFlowDefaults {
      get { return EMPTY; }
    }
    protected override String[] LinqDefaults {
      get { return EMPTY; }
    }
    protected override String[] VisibilityDefaults {
      get { return EMPTY; }
    }
    public override String KeyName {
      get { return Constants.Json; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType }; }
    }

    [ImportingConstructor]
    public JSON(IVsfSettings settings) : base(settings) {
    }

    public override IBraceScanner NewBraceScanner() {
      return new JScriptBraceScanner();
    }
  }
}
