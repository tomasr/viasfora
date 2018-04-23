using System;

namespace Winterdom.Viasfora.Settings {
  public interface ISettingsStore {
    String Get(String name);
    void Set(String name, String value);
    void Load();
    void Save();
  }
}
