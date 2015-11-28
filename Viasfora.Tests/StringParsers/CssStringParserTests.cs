using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringParsers {
  public class CssStringParserTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "'some string'";
      var parser = new CssStringParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void SimpleEscapeSequenceIsExtracted() {
      String input = @"'some\rstring'";
      var parser = new CssStringParser(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoEscapeSequencesAreExtracted() {
      String input = @"'some\r\nstring'";
      var parser = new CssStringParser(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void HexEscapeSequenceLen1IsExtracted() {
      String input = @"'some\enstring'";
      var parser = new CssStringParser(input);
      Assert.Equal(new StringPart(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void HexEscapeSequenceLen2IsExtracted() {
      String input = @"'some\e1nstring'";
      var parser = new CssStringParser(input);
      Assert.Equal(new StringPart(5,3), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void HexEscapeSequenceLen6IsExtracted() {
      String input = @"'some\123456eabfnstring'";
      var parser = new CssStringParser(input);
      Assert.Equal(new StringPart(5,7), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
