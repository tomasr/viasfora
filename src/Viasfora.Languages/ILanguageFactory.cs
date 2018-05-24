using System;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Languages {
  public interface ILanguageFactory {
    IEnumerable<ILanguage> GetAllLanguages();
    ILanguage TryCreateLanguage(Func<String, bool> contentTypeMatcher);
    ILanguage TryCreateLanguage(String key);
  }
}
