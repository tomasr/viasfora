using System;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Contracts {
  public interface ILanguage {
    ILanguageSettings Settings { get; }
    T GetService<T>();
    IStringScanner NewStringScanner(String classificationName, String text);
    bool MatchesContentType(IContentType contentType);
    bool IsKeywordClassification(IClassificationType classificationType);
    Func<String, String> NormalizationFunction { get; }
  }
}
