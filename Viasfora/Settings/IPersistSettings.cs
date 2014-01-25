using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Settings {
  public interface IPersistSettings {
    void Write(byte[] data);
    byte[] Read();
  }
}
