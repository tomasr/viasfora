using System;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringScanners {
  public class BasicCStringScannerTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "\"some string\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void OneEscapeSequenceIsExtracted() {
      String input = "\"" + @"some\rstring" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoContigousEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\r\nstring" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void EscapedBackslashIsExtractedCorrectly() {
      String input = "\"" + @"some\\string" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoSeparateEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\rother\nstring" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(12,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void EscapeInSingleQuotesIsExtracted() {
      String input = @"'some\rstring'";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NoSequencesInAtStringAreExtracted() {
      String input = "@\"" + @"some\rother\nstring" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void X1EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\x1string" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void X4EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\x1234string" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void U4EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\uABCDstring" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void U8EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\UABCD1234string" + "\"";
      var parser = new BasicCStringScanner(input);
      Assert.Equal(new StringPart(5,10), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
