using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceExtractors;
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
    protected override String KeyName {
      get { return "JSON"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new JScriptBraceExtractor(this);
    }
  }
}
