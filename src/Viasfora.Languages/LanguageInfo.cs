using System;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  public abstract class LanguageInfo {
    public T GetService<T>() {
      if ( typeof(T)  == typeof(IBraceScanner) ) {
        return (T)NewBraceScanner();
      } else if ( typeof(T) == typeof(IFirstLineCommentParser) ) {
        return (T)NewFirstLineCommentParser();
      }
      return default(T);
    }

    protected abstract IBraceScanner NewBraceScanner();
    protected virtual IFirstLineCommentParser NewFirstLineCommentParser() {
      return new GenericCommentParser();
    }
    public virtual IStringScanner NewStringScanner(String classificationName, String text) {
      return null;
    }

    public virtual bool MatchesContentType(Func<String, bool> contentTypeMatches) {
      foreach ( String str in this.SupportedContentTypes ) {
        if ( contentTypeMatches(str) ) 
          return true;
      }
      return false;
    }

    public virtual bool IsKeywordClassification(String classificationType) {
      return CompareClassification(classificationType, "Keyword");
    }

    public Func<String, String> NormalizationFunction { get; protected set; }

    protected abstract String[] SupportedContentTypes { get; }

    protected bool CompareClassification(String classificationType, String name) {
      return classificationType.Equals(name, StringComparison.OrdinalIgnoreCase);
    }

    public LanguageInfo() {
      this.NormalizationFunction = x => x;
    }
  }
}
