using System;
using Newtonsoft.Json;

namespace Winterdom.Viasfora.Settings {
  public interface ISettingsObject {
    String Name { get; }
    void Read(JsonTextReader reader);
    void Save(JsonTextWriter writer);
  }
}
