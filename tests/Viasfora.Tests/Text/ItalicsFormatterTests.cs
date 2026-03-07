using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.Text.Formatting;
using Winterdom.Viasfora.Text;
using Xunit;

namespace Viasfora.Tests.Text {
  public class ItalicsFormatterTests {

    private static ItalicsFormatter CreateFormatter(
      FakeClassificationFormatMap formatMap,
      FakeVsfSettings settings,
      List<string> classificationTypes
    ) {
      // Bypass the constructor (which needs ITextView) by creating
      // an uninitialized instance and setting fields via reflection.
      var formatter = (ItalicsFormatter)FormatterServices.GetUninitializedObject(typeof(ItalicsFormatter));
      var type = typeof(ItalicsFormatter);
      type.GetField("formatMap", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, formatMap);
      type.GetField("settings", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, settings);
      type.GetField("classificationTypes", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, classificationTypes);
      type.GetField("working", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(formatter, false);
      return formatter;
    }

    [Fact]
    public void FixIt_WhenItalicsDisabled_AndTextNotItalic_ShouldNotCallSetTextProperties() {
      // Text is already non-italic and setting is disabled → pure no-op
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { FlowControlUseItalics = false };
      var classifType = new FakeClassificationType("Viasfora Flow Control Keyword");
      var nonItalicProps = TextFormattingRunProperties.CreateTextFormattingRunProperties();
      formatMap.AddEntry(classifType, nonItalicProps);

      var formatter = CreateFormatter(formatMap, settings,
        new List<string> { "Viasfora Flow Control Keyword" });

      formatter.FixIt();

      Assert.Equal(0, formatMap.SetTextPropertiesCallCount);
    }

    [Fact]
    public void FixIt_WhenItalicsDisabled_AndTextIsItalic_ShouldRemoveItalics() {
      // Text IS italic but setting says don't use italics → should remove
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { FlowControlUseItalics = false };
      var classifType = new FakeClassificationType("Viasfora Flow Control Keyword");
      var italicProps = TextFormattingRunProperties.CreateTextFormattingRunProperties()
        .SetItalic(true);
      formatMap.AddEntry(classifType, italicProps);

      var formatter = CreateFormatter(formatMap, settings,
        new List<string> { "Viasfora Flow Control Keyword" });

      formatter.FixIt();

      Assert.Equal(1, formatMap.SetTextPropertiesCallCount);
      Assert.False(formatMap.LastSetProperties.Italic);
    }

    [Fact]
    public void FixIt_WhenItalicsEnabled_AndTextNotItalic_ShouldAddItalics() {
      // Text is not italic, setting says use italics → should add
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { FlowControlUseItalics = true };
      var classifType = new FakeClassificationType("Viasfora Flow Control Keyword");
      var nonItalicProps = TextFormattingRunProperties.CreateTextFormattingRunProperties();
      formatMap.AddEntry(classifType, nonItalicProps);

      var formatter = CreateFormatter(formatMap, settings,
        new List<string> { "Viasfora Flow Control Keyword" });

      formatter.FixIt();

      Assert.Equal(1, formatMap.SetTextPropertiesCallCount);
      Assert.True(formatMap.LastSetProperties.Italic);
    }

    [Fact]
    public void FixIt_WhenItalicsEnabled_AndTextAlreadyItalic_ShouldNotCallSetTextProperties() {
      // Text is already italic and setting is enabled → no change needed
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { FlowControlUseItalics = true };
      var classifType = new FakeClassificationType("Viasfora Flow Control Keyword");
      var italicProps = TextFormattingRunProperties.CreateTextFormattingRunProperties()
        .SetItalic(true);
      formatMap.AddEntry(classifType, italicProps);

      var formatter = CreateFormatter(formatMap, settings,
        new List<string> { "Viasfora Flow Control Keyword" });

      formatter.FixIt();

      Assert.Equal(0, formatMap.SetTextPropertiesCallCount);
    }

    [Fact]
    public void FixIt_NonMatchingClassificationType_ShouldNotCallSetTextProperties() {
      // Classification type doesn't match what formatter tracks → ignore
      var formatMap = new FakeClassificationFormatMap();
      var settings = new FakeVsfSettings { FlowControlUseItalics = true };
      var classifType = new FakeClassificationType("Some Other Type");
      var nonItalicProps = TextFormattingRunProperties.CreateTextFormattingRunProperties();
      formatMap.AddEntry(classifType, nonItalicProps);

      var formatter = CreateFormatter(formatMap, settings,
        new List<string> { "Viasfora Flow Control Keyword" });

      formatter.FixIt();

      Assert.Equal(0, formatMap.SetTextPropertiesCallCount);
    }
  }
}
