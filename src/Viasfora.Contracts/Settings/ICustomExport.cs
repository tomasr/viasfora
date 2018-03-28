using System;
using System.Collections.Generic;

namespace Winterdom.Viasfora.Settings {
  public interface ICustomExport {
    IDictionary<String, object> Export();
  }
}
