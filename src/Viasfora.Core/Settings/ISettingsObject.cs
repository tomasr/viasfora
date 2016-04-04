using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Winterdom.Viasfora.Settings {
  public interface ISettingsObject {
    String Name { get; }
    void Read(JsonTextReader reader);
    void Save(JsonTextWriter writer);
  }
}
