using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public abstract class LanguageInfo {
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

    public abstract String BraceList { get; }
    public abstract bool IsSingleLineCommentStart(String text, int pos);
    public abstract bool IsMultiLineCommentStart(String text, int pos);
    public abstract bool IsMultiLineCommentEnd(String text, int pos);
    public abstract bool IsSingleLineStringStart(String text, int pos, out char quote);
    public abstract bool IsMultiLineStringStart(String text, int pos, out char quote);
    public abstract bool IsStringEnd(String text, int pos, char quote);

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
