using System;

namespace Winterdom.Viasfora {
  public interface IUpdatableSettings {
    event EventHandler SettingsChanged;
  }
}
