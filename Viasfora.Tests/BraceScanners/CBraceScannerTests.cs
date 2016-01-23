using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class CBraceScannerTests : BaseScannerTests {

    [Fact]
    public void CanExtractParens() {
      String input = @"(x*(y+7))";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBrackets() {
      String input = @"x[y[0]]";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBraces() {
      String input = @"if ( true ) { }";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInSingleLineComment() {
      String input = @"
callF(1);
// callCommented(2);
";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInMultilineComment() {
      String input = @"
/* callF(1);
callCommented2(4);
*/
";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInString() {
      String input = "callF(\"some (string)\")";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInCharLiteral() {
      String input = "callF(']')";
      var extractor = new CBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
  }
}
