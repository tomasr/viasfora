using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Contracts {
  public interface ILanguageFactory {
    ILanguage TryCreateLanguage(IContentType contentType);
    ILanguage TryCreateLanguage(ITextBuffer textBuffer);
    ILanguage TryCreateLanguage(ITextSnapshot snapshot);
    ILanguage TryCreateLanguage(String key);
  }
}
