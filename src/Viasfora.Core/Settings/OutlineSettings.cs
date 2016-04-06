using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Winterdom.Viasfora.Settings {
  public class OutlineSettings : ISettingsObject {
    public string Name {
      get { return "outlines"; }
    }
    public List<Tuple<int, int>> Regions = new List<Tuple<int, int>>();

    public void Read(JsonTextReader reader) {
      Regions.Clear();
      if ( !reader.ReadStartObject() ) return;
      if ( reader.ReadPropertyName() != "regions" ) return;
      if ( !reader.ReadStartArray() ) return;

      while ( reader.ReadStartObject() ) {
        if ( reader.ReadPropertyName() != "start" ) break;
        int start = reader.ReadAsInt32().Value;

        if ( reader.ReadPropertyName() != "length" ) break;
        int end = reader.ReadAsInt32().Value;
        reader.ReadEndObject();

        Tuple<int, int> region = new Tuple<int,int>(start, end);
        Regions.Add(region);
      }
    }

    public void Save(JsonTextWriter writer) {
      writer.WriteStartObject();
      writer.WritePropertyName("regions");
      writer.WriteStartArray();
      foreach ( var region in Regions ) {
        writer.WriteStartObject();
        writer.WritePropertyName("start");
        writer.WriteValue(region.Item1);
        writer.WritePropertyName("length");
        writer.WriteValue(region.Item2);
        writer.WriteEndObject();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }
  }
}
