using System;
using Newtonsoft.Json;
using Xunit;
using Winterdom.Viasfora.Settings;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Viasfora.Tests.Settings {
  public class OutlineSettingsTests {

    [Fact]
    public void CanRead() {
      String json = @"
        { 'regions': [
          { 'start': 10, 'length': 20 },
          { 'start': 45, 'length': 30 }
        ]}";
      using ( var reader = NewReader(json) ) {
        OutlineSettings os = new OutlineSettings();
        os.Read(reader);
        Assert.Equal(2, os.Regions.Count);
        Assert.Equal(10, os.Regions[0].Item1);
        Assert.Equal(30, os.Regions[1].Item2);
      }
    }

    [Fact]
    public void CanWrite() {
      var writer = new StringWriter();
      using ( var jw = new JsonTextWriter(writer) ) {
        OutlineSettings os = new OutlineSettings();
        os.Regions.Add(new Tuple<int, int>(10, 32));
        os.Regions.Add(new Tuple<int, int>(27, 15));
        os.Save(jw);
      }
      var jo = JObject.Parse(writer.ToString());
      Assert.Equal(10, (int)jo["regions"][0]["start"]);
      Assert.Equal(15, (int)jo["regions"][1]["length"]);
    }

    private JsonTextReader NewReader(String json) {
      return new JsonTextReader(new StringReader(json));
    }
  }
}
