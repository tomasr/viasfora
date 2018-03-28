using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Contracts {
  public interface ILanguageFactory {
    IEnumerable<ILanguage> GetAllLanguages();
    ILanguage TryCreateLanguage(Func<String, bool> contentTypeMatcher);
    ILanguage TryCreateLanguage(String key);
  }
}
