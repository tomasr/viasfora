using System;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  public interface ILanguage {
    ILanguageSettings Settings { get; }
    T GetService<T>();
    IStringScanner NewStringScanner(String classificationName, String text);
    bool MatchesContentType(Func<String, bool> contentTypeMatches);
    bool IsKeywordClassification(String classificationType);
    Func<String, String> NormalizationFunction { get; }
  }
}
