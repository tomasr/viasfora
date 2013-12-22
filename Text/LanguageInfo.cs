using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {
  public abstract class LanguageInfo {
    private static StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;

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
    public abstract bool SupportsEscapeSeqs { get; }
    public abstract IBraceExtractor NewBraceExtractor();

    public bool MatchesContentType(IContentType contentType) {
      foreach ( String str in this.ContentTypes ) {
        if ( contentType.IsOfType(str) ) 
          return true;
      }
      return false;
    }

    public bool IsControlFlowKeyword(String text) {
      return ControlFlow.Contains(text, comparer);
    }
    public bool IsVisibilityKeyword(String text) {
      return Visibility.Contains(text, comparer);
    }
    public bool IsLinqKeyword(String text) {
      return Linq.Contains(text, comparer);
    }

    protected abstract String[] ContentTypes { get; }
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
