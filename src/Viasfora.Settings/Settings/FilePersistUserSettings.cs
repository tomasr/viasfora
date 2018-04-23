using System;
using System.IO;

namespace Winterdom.Viasfora.Settings {
  public class FilePersistUserSettings : IPersistSettings {
    public const String FILENAME = "settings.vsfuser";
    private String settingsFile;

    public FilePersistUserSettings(String location) {
      String path = Path.GetFullPath(location);
      if ( !String.IsNullOrEmpty(Path.GetExtension(path)) ) {
        path = Path.GetDirectoryName(path);
      }
      this.settingsFile = Path.Combine(path, FILENAME);
    }

    public void Write(byte[] data) {
      File.WriteAllBytes(this.settingsFile, data);
    }

    public byte[] Read() {
      if ( !SettingsFileExist() ) {
        return null;
      }
      return File.ReadAllBytes(this.settingsFile);
    }

    private bool SettingsFileExist() {
      return File.Exists(this.settingsFile);
    }
  }
}
