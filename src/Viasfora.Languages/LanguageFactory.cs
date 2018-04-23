using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

    public ILanguage TryCreateLanguage(Func<String, bool> contentTypeMatcher) {
      foreach ( ILanguage lang in Languages ) {
        if ( lang.MatchesContentType(contentTypeMatcher) ) {
          return lang;
        }
      }
      return defaultLang;
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
