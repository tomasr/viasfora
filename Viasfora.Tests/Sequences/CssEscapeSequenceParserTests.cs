using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;

namespace Viasfora.Tests.Sequences {
  public class CssEscapeSequenceParserTests {
    [Fact]
    public void NoEscapesReturnsNull() {
      String input = "'some string'";
      var parser = new CssEscapeSequenceParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void SimpleEscapeSequenceIsExtracted() {
      String input = @"'some\rstring'";
      var parser = new CssEscapeSequenceParser(input);
      Assert.Equal(new Span(5,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
  }
}
