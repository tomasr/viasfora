using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Linq;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora {
  public static class LanguageExtensions {
    private static StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;

    public static bool IsControlFlowKeyword(this ILanguage lang, String text) {
      return lang.Settings.ControlFlow.Contains(lang.NormalizationFunction(text), comparer);
    }
    public static bool IsVisibilityKeyword(this ILanguage lang, String text) {
      return lang.Settings.Visibility.Contains(lang.NormalizationFunction(text), comparer);
    }
    public static bool IsLinqKeyword(this ILanguage lang, String text) {
      return lang.Settings.Linq.Contains(lang.NormalizationFunction(text), comparer);
    }

    public static ILanguage TryCreateLanguage(this ILanguageFactory factory, ITextBuffer buffer) {
      return factory.TryCreateLanguage(buffer.ContentType);
    }
    public static ILanguage TryCreateLanguage(this ILanguageFactory factory, ITextSnapshot snapshot) {
      return factory.TryCreateLanguage(snapshot.ContentType);
    }
    public static ILanguage TryCreateLanguage(this ILanguageFactory factory, IContentType contentType) {
      bool matcher(String lang) => contentType.IsOfType(lang);
      return factory.TryCreateLanguage(matcher);
    }
  }
}
