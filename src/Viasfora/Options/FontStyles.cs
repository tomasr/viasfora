using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Winterdom.Viasfora.Options {
  [Flags]
  [JsonConverter(typeof(StringEnumConverter))]
  public enum FontStyles {
    None = 0,
    Bold = 0x01,
    Strikethrough = 0x02,
    Italics = 0x04
  }
}
