using System;

namespace Winterdom.Viasfora.Settings {
  public interface ISettingsExport {
    void Export<T>(T settingsObject) where T : class;
    void Import(ISettingsStore store);
    void Load(String sourcePath);
    void Save(String targetPath);
  }
}
