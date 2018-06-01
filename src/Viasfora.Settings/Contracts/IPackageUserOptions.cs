using System;
using System.Runtime.InteropServices;

namespace Winterdom.Viasfora.Contracts {
  [Guid("19e5d71b-8817-4ac0-a667-4d3fbc4cb409")]
  public interface IPackageUserOptions {
    byte[] Read();
    void Write(byte[] options);
  }

  [Guid("75baf604-4e93-4254-b578-e3af261a625f")]
  public class SPackageUserOptions {
  }
}
