using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Winterdom.Viasfora.Settings;
using Xunit;

namespace Viasfora.Tests.Settings {
  public class VsfSettingsTests {
    [Fact]
    public void CanConvertDoubleUsingInvariantCulture() {
      var thread = Thread.CurrentThread;
      var originalCulture = thread.CurrentCulture;
      thread.CurrentCulture = new CultureInfo("es-co");
      try {
        var value = VsfSettings.ConvertToDouble("1.23");
        Assert.Equal(1.23, value);
      } finally {
        thread.CurrentCulture = originalCulture;
      }
    }
    [Fact]
    public void CanConvertDoubleUsingCurrentCultureAsLastResort() {
      var thread = Thread.CurrentThread;
      var originalCulture = thread.CurrentCulture;
      thread.CurrentCulture = new CultureInfo("es-co");
      try {
        var value = VsfSettings.ConvertToDouble("987.991,23");
        Assert.Equal(987991.23, value);
      } finally {
        thread.CurrentCulture = originalCulture;
      }
    }
  }
}
