using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Winterdom.Viasfora {
  public static class JsonExtensions {
    public static bool ReadStartObject(this JsonTextReader reader) {
      return reader.ReadExpected(JsonToken.StartObject);
    }
    public static bool ReadEndObject(this JsonTextReader reader) {
      return reader.ReadExpected(JsonToken.EndObject);
    }
    public static bool ReadStartArray(this JsonTextReader reader) {
      return reader.ReadExpected(JsonToken.StartArray);
    }
    public static String ReadPropertyName(this JsonTextReader reader) {
      if ( reader.ReadExpected(JsonToken.PropertyName) ) {
        return reader.Value as String;
      }
      return null;
    }

    private static bool ReadExpected(this JsonTextReader reader, JsonToken expected) {
      if ( reader.Read() ) {
        return reader.TokenType == expected;
      }
      return false;
    }
  }
}
