using System;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringScanners {
  public class PsStringScannerTests {
    //
    // Single Quoted Strings
    //
    [Fact]
    public void SQ_EmptyReturnsNull() {
      String input = "''";
      var parser = new PsStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void SQ_EscapesAreIgnored() {
      String input = "'some`rstring'";
      var parser = new PsStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void SQ_HS_EmbeddedQuotes() {
      String input = "@'\r\n'" + @"some`nstring" + "'\r\n'@";
      var parser = new PsStringScanner(input);
      Assert.Equal(null, parser.Next());
    }

    //
    // Double Quote Strings
    //
    [Fact]
    public void DQ_EmptyReturnsNull() {
      String input = "\"" + @"" + "\"";
      var parser = new PsStringScanner(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_OneSequence() {
      String input = "\"" + @"some`rstring" + "\"";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_TwoSequencesContigous() {
      String input = "\"" + @"some`r`nstring" + "\"";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_TwoSequencesNotContigous() {
      String input = "\"" + @"some`rother`nstring" + "\"";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(12,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_EscapedBacktick() {
      String input = "\"" + @"some``nstring" + "\"";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_TwoEscapedBackticksTogether() {
      String input = "\"" + @"some````nstring" + "\"";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_HS_OneEscape() {
      String input = "@\"\r\n" + @"some`nstring" + "\r\n\"@";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(8,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void DQ_HS_EmbeddedQuotes() {
      String input = "@\"\r\n\"" + @"some`nstring" + "\"\r\n\"@";
      var parser = new PsStringScanner(input);
      Assert.Equal(new StringPart(9,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
