using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.StringScanners {
  public class RStringScannerTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "\"some string\"";
      var parser = new RStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void OneEscapeSequenceIsExtracted() {
      String input = "\"" + @"some\rstring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoContigousEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\r\nstring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void EscapedBackslashIsExtractedCorrectly() {
      String input = "\"" + @"some\\string" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoSeparateEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\rother\nstring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(12,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void EscapeInSingleQuotesIsExtracted() {
      String input = @"'some\rstring'";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void X1EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\x1string" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void X2EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\x1astring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,4), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void U1EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\uAstring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void U4EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\uABCDstring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UBracesEscapeSequenceIsExtracted() {
      String input = "\"" + @"some\u{A12}string" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,7), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UpperU1EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\UAstring" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UpperU8EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\UABCD1234string" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,10), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UpperUBracesEscapeSequenceIsExtracted() {
      String input = "\"" + @"some\U{A12}string" + "\"";
      var parser = new RStringScanner(input);
      Assert.Equal(new StringPart(5,7), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
