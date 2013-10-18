using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  abstract class LanguageKeywords {
    private Dictionary<string, string[]> keywords =
       new Dictionary<string, string[]>();

    public String[] ControlFlow {
      get { return Get("ControlFlow", ControlFlowDefaults); }
    }
    public String[] Linq {
      get { return Get("Linq", LinqDefaults); }
    }
    public String[] Visibility {
      get { return Get("Visibility", VisibilityDefaults); }
    }

    protected abstract String[] ControlFlowDefaults { get; }
    protected abstract String[] LinqDefaults { get; }
    protected abstract String[] VisibilityDefaults { get; }
    protected abstract String KeyName { get; }

    protected String[] Get(String name, String[] defaults) {
      if ( !keywords.ContainsKey(name) ) {
        String[] values =
           ConfigHelp.GetValue(KeyName + "_" + name, "").AsList();
        if ( values == null || values.Length == 0 )
          values = defaults;
        keywords[name] = values;
      }
      return keywords[name];
    }
  }
}
