using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Winterdom.Viasfora.Settings {
  public class SuoPersistUserSettings : IPersistSettings {
    private VsfPackage package;

    public SuoPersistUserSettings(VsfPackage package) {
      this.package = package;
    }

    public void Write(byte[] data) {
      package.UserOptions = data;
    }

    public byte[] Read() {
      return package.UserOptions;
    }
  }
}
