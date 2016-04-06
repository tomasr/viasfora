using System;

namespace Winterdom.Viasfora.Contracts {
  public interface IPackageUserOptions {
    byte[] Read();
    void Write(byte[] options);
  }
}
