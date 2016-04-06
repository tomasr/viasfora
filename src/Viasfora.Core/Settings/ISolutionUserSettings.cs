using System;

namespace Winterdom.Viasfora.Settings {
  // represents a mechanism for storing user settings associated
  // with the current solution
  public interface ISolutionUserSettings {
    void Store<T>(String filePath, T settingsObject) where T : ISettingsObject;
    T Load<T>(String filePath) where T : ISettingsObject, new();
  }
}
