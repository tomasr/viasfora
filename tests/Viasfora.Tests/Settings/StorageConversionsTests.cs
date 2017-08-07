using System;
using System.Globalization;
using System.Threading;
using Winterdom.Viasfora.Settings;
using Xunit;

namespace Viasfora.Tests.Settings {
  public class StorageConversionsTests {
    [Fact]
    public void CanConvertDoubleUsingInvariantCulture() {
      var converter = new StorageConversions();
      var thread = Thread.CurrentThread;
      var originalCulture = thread.CurrentCulture;
      thread.CurrentCulture = new CultureInfo("es-co");
      try {
        var value = converter.ToDouble("1.23");
        Assert.Equal(1.23, value);
      } finally {
        thread.CurrentCulture = originalCulture;
      }
    }
    [Fact]
    public void CanConvertDoubleUsingCurrentCultureAsLastResort() {
      var converter = new StorageConversions();
      var thread = Thread.CurrentThread;
      var originalCulture = thread.CurrentCulture;
      thread.CurrentCulture = new CultureInfo("es-co");
      try {
        var value = converter.ToDouble("987.991,23");
        Assert.Equal(987991.23, value);
      } finally {
        thread.CurrentCulture = originalCulture;
      }
    }
    [Fact]
    public void CanConvertInt32UsingInvariantCulture() {
      var converter = new StorageConversions();
      var thread = Thread.CurrentThread;
      var originalCulture = thread.CurrentCulture;
      thread.CurrentCulture = new CultureInfo("es-co");
      try {
        var value = converter.ToInt32("123");
        Assert.Equal(123, value);
      } finally {
        thread.CurrentCulture = originalCulture;
      }
    }
    [Fact]
    public void CanConvertInt64UsingInvariantCulture() {
      var converter = new StorageConversions();
      var thread = Thread.CurrentThread;
      var originalCulture = thread.CurrentCulture;
      thread.CurrentCulture = new CultureInfo("es-co");
      try {
        var value = converter.ToInt64("123");
        Assert.Equal(123, value);
      } finally {
        thread.CurrentCulture = originalCulture;
      }
    }
  }
}
