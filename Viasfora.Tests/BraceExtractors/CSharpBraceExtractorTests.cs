using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.BraceExtractors {
  public class CSharpBraceExtractorTests {

    [Fact]
    public void CanExtractParens() {
      String input = @"(x*(y+7))";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBrackets() {
      String input = @"x[y[0]]";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBraces() {
      String input = @"if ( true ) { }";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInSingleLineComment() {
      String input = @"
callF(1);
// callCommented(2);
";
      var extractor = new CSharpBraceExtractor();
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
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInString() {
      String input = "callF(\"some (string)\")";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInAtString() {
      String input = "callF(@\"some (string)\")";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInCharLiteral() {
      String input = "callF(']')";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    //
    // C# 6.0 features
    //
    [Fact]
    public void InterpolatedString1() {
      String input = "$\"some {site} other\"";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithDoubleBraces() {
      String input = "$\"first is not {{interpolated}} other is {interpolated}\"";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithDoubleBraces2() {
      String input = "$\"first is {{{interpolated}}} other is {interpolated}\"";
      var extractor = new CSharpBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    private IList<CharPos> Extract(IBraceExtractor extractor, string input, int start, int state) {
      extractor.Reset();
      ITextChars chars = new StringChars(input, start);
      return extractor.Extract(chars).ToList();
    }
  }
}
