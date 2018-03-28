using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguageFactory))]
  public class LanguageFactory : ILanguageFactory {
    [ImportMany]
    public List<ILanguage> Languages { get; set; }
    private ILanguage defaultLang;

    [ImportingConstructor]
    public LanguageFactory(ITypedSettingsStore store) {
      this.defaultLang = new DefaultLanguage(store);
    }

    public IEnumerable<ILanguage> GetAllLanguages() {
      return this.Languages;
    }

    public ILanguage TryCreateLanguage(IContentType contentType) {
      bool matches(string ct) => contentType.IsOfType(ct);

      foreach ( ILanguage lang in Languages ) {
        if ( lang.MatchesContentType(matches) ) {
          return lang;
        }
      }
      return defaultLang;
    }

    public ILanguage TryCreateLanguage(ITextBuffer textBuffer) {
      return TryCreateLanguage(textBuffer.ContentType);
    }
    public ILanguage TryCreateLanguage(ITextSnapshot snapshot) {
      return TryCreateLanguage(snapshot.ContentType);
    }
    public ILanguage TryCreateLanguage(String key) {
      foreach ( ILanguage lang in Languages ) {
        if ( lang.Settings.KeyName == key ) {
          return lang;
        }
      }
      return defaultLang;
    }
  }
}
