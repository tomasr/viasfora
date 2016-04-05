using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora {
  public interface IUpdatableSettings {
    event EventHandler SettingsChanged;
  }
}
