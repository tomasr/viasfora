using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class RBraceScannerTests : BaseScannerTests {
    [Fact]
    public void CanExtractParens() {
      String input = @"(x*(y+7))";
      var extractor = new RBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBrackets() {
      String input = @"x[y[0]]";
      var extractor = new RBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBraces() {
      String input = @"for ( a in args ) {call1(a); call2(b) }";
      var extractor = new RBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(8, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInSingleLineComment() {
      String input = @"
callF(1);
# callCommented(2);
";
      var extractor = new RBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInString() {
      String input = "callF(\"some (string)\")";
      var extractor = new RBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInSingleQuotedString() {
      String input = "callF('some(string)')";
      var extractor = new RBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
  }
}
