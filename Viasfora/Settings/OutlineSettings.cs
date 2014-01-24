using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Winterdom.Viasfora.Settings {
  public class OutlineSettings : ISettingsObject {
    public string Name {
      get { return "outlines"; }
    }
    public List<Tuple<int, int>> Regions = new List<Tuple<int, int>>();

    public void Read(JsonTextReader reader) {
      Regions.Clear();
      reader.Read(); // read start object
      reader.Read(); // property name
      if ( (String)reader.Value != "regions" ) return;
      reader.Read(); // read start array
      while ( reader.Read() ) {
        reader.Read(); // property name
        if ( (String)reader.Value != "start" ) break;
        int start = reader.ReadAsInt32().Value;
        reader.Read(); // property name
        if ( (String)reader.Value != "length" ) break;
        int end = reader.ReadAsInt32().Value;
        reader.Read(); // end object

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
