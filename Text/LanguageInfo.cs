using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  abstract class LanguageInfo {
    public String[] ControlFlow {
      get { return Get("ControlFlow", ControlFlowDefaults); }
      set { Set("ControlFlow", value); }
    }
    public String[] Linq {
      get { return Get("Linq", LinqDefaults); }
      set { Set("Linq", value); }
    }
    public String[] Visibility {
      get { return Get("Visibility", VisibilityDefaults); }
      set { Set("Visibility", value); }
    }

    protected abstract String[] ControlFlowDefaults { get; }
    protected abstract String[] LinqDefaults { get; }
    protected abstract String[] VisibilityDefaults { get; }
    protected abstract String KeyName { get; }

    protected String[] Get(String name, String[] defaults) {
      String[] values = VsfSettings.GetValue(this.KeyName + "_" + name, null).AsList();
      if ( values == null || values.Length == 0 )
        values = defaults;
      return values;
    }
    protected void Set(String name, IEnumerable<String> values) {
      VsfSettings.SetValue(this.KeyName + "_" + name, values.FromList());
    }
  }
}
