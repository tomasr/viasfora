using System;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringScanners {
  [Collection("DependsOnVS")]
  public class MplStringScannerTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "\"some string\"";
      var parser = new MplStringScanner(input);
      Assert.Equal(null, parser.Next());
    }

    [Fact]
    public void OneEscapeSequenceIsExtracted() {
      String input = "\"" + @"some\\string" + "\"";
      var parser = new MplStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }

    [Fact]
    public void OneEscapeSequenceIsExtracted2() {
      String input = "\"some\\\"string\"";
      var parser = new MplStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }

    [Fact]
    public void TwoContigousEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\\\\string" + "\"";
      var parser = new MplStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }

    [Fact]
    public void TwoSeparateEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\\other\\string" + "\"";
      var parser = new MplStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(12,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
