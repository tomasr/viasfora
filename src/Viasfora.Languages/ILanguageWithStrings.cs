using System;

namespace Winterdom.Viasfora.Languages {
  public interface ILanguageWithStrings : ILanguage {
    bool IsStringClassification(String classificationType);
  }
}
