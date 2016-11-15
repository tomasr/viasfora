using System;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringScanners {
  [Collection("DependsOnVS")]
  public class CssStringScannerTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "'some string'";
      var parser = new CssStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void SimpleEscapeSequenceIsExtracted() {
      String input = @"'some\rstring'";
      var parser = new CssStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoEscapeSequencesAreExtracted() {
      String input = @"'some\r\nstring'";
      var parser = new CssStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void HexEscapeSequenceLen1IsExtracted() {
      String input = @"'some\enstring'";
      var parser = new CssStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void HexEscapeSequenceLen2IsExtracted() {
      String input = @"'some\e1nstring'";
      var parser = new CssStringScanner(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void HexEscapeSequenceLen6IsExtracted() {
      String input = @"'some\123456eabfnstring'";
      var parser = new CssStringScanner(input);
      Assert.Equal(new StringPart(5,7), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
