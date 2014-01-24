using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Winterdom.Viasfora.Settings;
using System.IO;

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


    private JsonTextReader NewReader(String json) {
      return new JsonTextReader(new StringReader(json));
    }
  }
}
