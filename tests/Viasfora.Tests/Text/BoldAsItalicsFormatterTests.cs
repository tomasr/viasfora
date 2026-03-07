using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Formatting;
using Winterdom.Viasfora.Text;
using Xunit;

namespace Viasfora.Tests.Text {
  public class BoldAsItalicsFormatterTests {

    private static BoldAsItalicsFormatter CreateFormatter(
      FakeClassificationFormatMap formatMap,
      FakeVsfSettings settings
    ) {
      var formatter = (BoldAsItalicsFormatter)FormatterServices.GetUninitializedObject(typeof(BoldAsItalicsFormatter));
      var type = typeof(BoldAsItalicsFormatter);
      type.GetField("formatMap", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, formatMap);
      type.GetField("settings", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, settings);
      type.GetField("working", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, false);
      return formatter;
    }

    private static bool GetWorkingFlag(BoldAsItalicsFormatter formatter) {
      return (bool)typeof(BoldAsItalicsFormatter)
        .GetField("working", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(formatter);
    }

    private static void InvokeMakeBoldItalics(BoldAsItalicsFormatter formatter) {
      typeof(BoldAsItalicsFormatter)
        .GetMethod("MakeBoldItalics", BindingFlags.NonPublic | BindingFlags.Instance)
        .Invoke(formatter, null);
    }

    [Fact]
    public void MakeBoldItalics_WhenEnabled_WorkingFlagShouldRemainTrueAfterReturn() {
      // The working flag must NOT be reset immediately; it should use
      // a delayed reset (Task.Delay) to prevent re-entrant cascading.
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { BoldAsItalicsEnabled = true };
      var formatter = CreateFormatter(formatMap, settings);

      InvokeMakeBoldItalics(formatter);

      Assert.True(GetWorkingFlag(formatter));
    }

    [Fact]
    public void MakeBoldItalics_WhenEnabled_SecondCallShouldBeBlockedByWorkingFlag() {
      // After the first call, a second immediate call should be blocked
      // because the working flag hasn't been reset yet.
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { BoldAsItalicsEnabled = true };
      var classifType = new FakeClassificationType("keyword");
      var boldProps = TextFormattingRunProperties.CreateTextFormattingRunProperties(
        new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
        12.0, Colors.Black).SetBold(true);
      formatMap.AddEntry(classifType, boldProps);

      var formatter = CreateFormatter(formatMap, settings);

      // First call processes the bold→italic conversion
      InvokeMakeBoldItalics(formatter);
      int callsAfterFirst = formatMap.SetTextPropertiesCallCount;

      // Reset the entry back to bold to ensure the second call WOULD
      // do work if it weren't blocked
      formatMap.AddEntry(classifType, boldProps);
      formatMap.SetTextPropertiesCallCount = 0;

      // Second call should be blocked by the working flag
      InvokeMakeBoldItalics(formatter);

      Assert.Equal(0, formatMap.SetTextPropertiesCallCount);
    }

    [Fact]
    public void MakeBoldItalics_WhenDisabled_ShouldNotProcess() {
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { BoldAsItalicsEnabled = false };
      var formatter = CreateFormatter(formatMap, settings);

      InvokeMakeBoldItalics(formatter);

      Assert.Equal(0, formatMap.SetTextPropertiesCallCount);
      Assert.False(GetWorkingFlag(formatter));
    }
  }
}
