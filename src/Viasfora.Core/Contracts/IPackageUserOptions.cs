using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Contracts {
  public interface IPackageUserOptions {
    byte[] Read();
    void Write(byte[] options);
  }
}
