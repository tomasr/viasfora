using System;

namespace Winterdom.Viasfora.Settings {
  public interface IPersistSettings {
    void Write(byte[] data);
    byte[] Read();
  }
}
