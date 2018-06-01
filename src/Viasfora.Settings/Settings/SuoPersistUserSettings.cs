using System;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Settings {
  public class SuoPersistUserSettings : IPersistSettings {
    private IPackageUserOptions userOptions;

    public SuoPersistUserSettings(IPackageUserOptions options) {
      this.userOptions = options;
    }

    public void Write(byte[] data) {
      this.userOptions.Write(data);
    }

    public byte[] Read() {
      return this.userOptions.Read();
    }
  }
}
