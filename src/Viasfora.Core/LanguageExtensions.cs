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
  }
}
