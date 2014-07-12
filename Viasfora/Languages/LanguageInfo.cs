using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Util;
using Winterdom.Viasfora.Languages.CommentParsers;

namespace Winterdom.Viasfora.Languages {
  public abstract class LanguageInfo : ILanguage {
    private static StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
    protected static readonly String[] EMPTY = { };

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
    public abstract IBraceExtractor NewBraceExtractor();
    public virtual IFirstLineCommentParser NewFirstLineCommentParser() {
      return new GenericCommentParser();
    }
    public virtual IEscapeSequenceParser NewEscapeSequenceParser(String text) {
      return null;
    }

    public bool MatchesContentType(IContentType contentType) {
      foreach ( String str in this.ContentTypes ) {
        if ( contentType.IsOfType(str) ) 
          return true;
      }
      return false;
    }

    public bool IsControlFlowKeyword(String text) {
      return ControlFlow.Contains(TextToCompare(text), comparer);
    }
    public bool IsVisibilityKeyword(String text) {
      return Visibility.Contains(TextToCompare(text), comparer);
    }
    public bool IsLinqKeyword(String text) {
      return Linq.Contains(TextToCompare(text), comparer);
    }

    protected virtual String TextToCompare(String text) {
      return text;
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
