using System;
using Winterdom.Viasfora.Compatibility;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora {
  public class SettingsContext {

    public static IVsfSettings GetSettings() {
      var model = new SComponentModel();
      return model.GetService<IVsfSettings>();
    }
    public static T GetService<T>() {
      var model = new SComponentModel();
      return model.GetService<T>();
    }
    public static ILanguage GetLanguage(String key) {
      var model = new SComponentModel();
      var factory = model.GetService<ILanguageFactory>();
      return factory.TryCreateLanguage(key);
    }
  }
}
