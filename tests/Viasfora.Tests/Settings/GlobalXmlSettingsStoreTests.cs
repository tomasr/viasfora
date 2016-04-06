using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Winterdom.Viasfora.Settings;
using Xunit;

namespace Viasfora.Tests.Settings {
  public class GlobalXmlSettingsStoreTests {
    [Fact]
    public void CanWriteSettings() {
      String path = Path.GetTempFileName();
      ISettingsStore store = new GlobalXmlSettingsStore(path);
      store.Set("test1", "Value 1");
      store.Set("test2", "Value 2");
      store.Save();

      Assert.True(File.Exists(path));
      XDocument doc = XDocument.Load(path);
      Assert.Equal(2, doc.Root.Elements().Count());
    }
    [Fact]
    public void CanWriteAndReadBackSettings() {
      String path = Path.GetTempFileName();
      ISettingsStore store = new GlobalXmlSettingsStore(path);
      store.Set("test1", "Value 1");
      store.Set("test2", "Value 2");
      store.Save();

      store = new GlobalXmlSettingsStore(path);
      store.Load();
      Assert.Equal("Value 1", store.Get("test1"));
    }
  }
}
