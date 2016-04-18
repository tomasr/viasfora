using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.StringScanners {

  public class CSharpStringScannerTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "\"some string\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void OneEscapeSequenceIsExtracted() {
      String input = "\"" + @"some\rstring" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoContigousEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\r\nstring" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void EscapedBackslashIsExtractedCorrectly() {
      String input = "\"" + @"some\\string" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoSeparateEscapeSequencesAreExtracted() {
      String input = "\"" + @"some\rother\nstring" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(12,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void EscapeInSingleQuotesIsExtracted() {
      String input = @"'some\rstring'";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NoSequencesInAtStringAreExtracted() {
      String input = "@\"" + @"some\rother\nstring" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NoSequencesInInterpolatedAtStringAreExtracted() {
      String input = "$@\"" + @"some\rother\nstring" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void X1EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\x1string" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void X4EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\x1234string" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void U4EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\uABCDstring" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void U8EscapeSequenceIsExtracted() {
      String input = "\"" + @"some\UABCD1234string" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(5,10), parser.Next());
      Assert.Equal(null, parser.Next());
    }

    //
    // Format Specifier Tests
    //
    [Fact]
    public void SimpleFormatSpecifierIsExtracted() {
      String input = "\"" + @"Hello {0}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(7, 3, StringPartType.FormatSpecifier), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void FormatSpecWithTypeIsExtracted() {
      String input = "\"" + @"Value: {0:c}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(8, 5, StringPartType.FormatSpecifier), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void FormatSpecWithTypeAndPrecisionIsExtracted() {
      String input = "\"" + @"Value: {0:x2}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(8, 6, StringPartType.FormatSpecifier), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void FormatSpecWithTypePrecisionAndAlignmentIsExtracted() {
      String input = "\"" + @"Value: {0,-3:x2}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(new StringPart(8, 9, StringPartType.FormatSpecifier), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void FormatSpecInDoubleBracesIsIgnored() {
      String input = "\"" + @"Value: {{0}}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void IncompleteSpecIsIgnored() {
      String input = "\"" + @"Value: {0" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void FormatSpecsInInterpolatedStringsAreIgnored() {
      String input = "$\"" + @"Value: {0}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void FormatSpecsInInterpolatedAtStringsAreIgnored() {
      String input = "$@\"" + @"Value: {0}" + "\"";
      var parser = new CSharpStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
  }
}
