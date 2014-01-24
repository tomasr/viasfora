using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Settings {
  // represents a mechanism for storing user settings associated
  // with the current solution
  public interface ISolutionUserSettings {
    void Store<T>(String filePath, T settingsObject) where T : ISettingsObject;
    T Load<T>(String filePath) where T : ISettingsObject, new();
    String MakeRelativePath(String filePath);
  }
}
