using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Contracts {
  public interface ILanguageFactory {
    IEnumerable<ILanguage> GetAllLanguages();
    ILanguage TryCreateLanguage(IContentType contentType);
    ILanguage TryCreateLanguage(ITextBuffer textBuffer);
    ILanguage TryCreateLanguage(ITextSnapshot snapshot);
    ILanguage TryCreateLanguage(String key);
  }
}
