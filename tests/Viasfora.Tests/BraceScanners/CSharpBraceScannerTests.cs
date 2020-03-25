using System;
using System.Collections.Generic;
using System.Linq;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class CSharpBraceScannerTests : BaseScannerTests {

    [Fact]
    public void CanExtractParens() {
      String input = @"(x*(y+7))";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBrackets() {
      String input = @"x[y[0]]";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void CanExtractBraces() {
      String input = @"if ( true ) { }";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInSingleLineComment() {
      String input = @"
callF(1);
// callCommented(2);
";
      var extractor = new CSharpBraceScanner();
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
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInString() {
      String input = "callF(\"some (string)\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInAtString() {
      String input = "callF(@\"some (string)\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInCharLiteral() {
      String input = "callF(']')";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    //
    // C# 6.0 features
    //
    [Fact]
    public void InterpolatedString1() {
      String input = "$\"some {site} other\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithDoubleBraces() {
      String input = "$\"first is not {{interpolated}} other is {interpolated}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithDoubleBraces2() {
      String input = "$\"first is {{{interpolated}}} other is {interpolated}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithNestedString() {
      String input = "$\"interpolated: {CallMethod(\"string\")}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithNestedCharLiteral() {
      String input = "$\"interpolated: {CallMethod(')')}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void InterpolatedStringWithFormatSpecifier() {
      String input = "$\"interpolated: {x : 08x}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void InterpolatedStringDoesNotReturnBracesInStringPart() {
      String input = "$\"Hello {username} on Math.Cos((r/0.122))}.\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void InterpolatedStringDoesNotReturnBracesInStringPart_Partial() {
      String input = "$\"Hello {username} on ";
      String input2 = "Math.Cos((r/0.122))}.\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
      // second part should not be changed
      chars = Extract(extractor, input2.Trim(), 0, 0, false);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void Bug123_InterpolatedStringInParens() {
      String input = "CallMe($\"Hello {username}.\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(1+2+1, chars.Count);
    }
    [Fact]
    public void Bug123_InterpolatedStringWithNestedString() {
      String input = "CallMe($\"Hello {ViewData[\"username\"]}.\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(1+2+2+1, chars.Count);
    }
    [Fact]
    public void Bug128_InterpolatedStringWithNestedCurlyBraces() {
      String input = "$\"{String.Concat(new[] {\"Hello\", \"World\"})}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(1+1+2+1+1+1+1, chars.Count);
    }
    [Fact]
    public void Bug259_InterpolatedStringEmbedded() {
      String input = "$\"{(string.IsNullOrWhiteSpace(a) ? $\"{b}\" : $\"{c}\")}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2+1+1+1+1+1+1+2, chars.Count);
    }

    public List<string> testList = new List<string>();

    [Fact]
    public void Bug263_InterpolatedStringWithInterpolatedString1() {

      // bug is the missing ) in following section of the teststring:
      // n => $\"pre_{n}_suf\").Aggregate

      String input = "string.IsNullOrEmpty($\"{test} {(string.IsNullOrEmpty(test) ? \"\" : testList.Select(n => $\"pre_{n}_suf\").Aggregate((n,m) =>  n + \", \" + m))}\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(1+1+1+2+1+1+1+1+1+1+2+1+3+1, chars.Count);
    }

    [Fact]
    public void Bug263_InterpolatedStringWithInterpolatedString2() {

      // bug is the missing ) in following section of the teststring:
      // n => $\"pre_{n}_suf\").Aggregate

      String input = "string.IsNullOrEmpty($\"{test} {(string.IsNullOrEmpty(test) ? \"\" : testList.Select(n => $@\"pre_{n}_suf\").Aggregate((n,m) =>  n + \", \" + m))}\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(1 + 1 + 1 + 2 + 1 + 1 + 1 + 1 + 1 + 1 + 2 + 1 + 3 + 1, chars.Count);
    }

    [Fact]
    public void InterpolatedAtString1() {
      String input = "$@\"some {site} other\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void InterpolatedAtString2() {
      String input = "$@\"some {site} other\r\n"
                   + "some {super} line\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void InterpolatedAtStringWithBackslash() {
      String input = "$@\"some {site}\\{another} other\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void InterpolatedAtStringWithDoubleQuotes() {
      String input = "$@\"class MyClass {{ Console.WriteLine(\"\"test\"\");\r\n}}\"";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    [Fact]
    public void InterpolatedStringNonRestartable1() {
      String input = "$\"some {s";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(1, chars.Count);
      Assert.NotEqual(0, chars[0].State);
    }
    [Fact]
    public void InterpolatedStringNonRestartable2() {
      String input = "$\"some {s.ToString()";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(3, chars.Count);
      Assert.NotEqual(0, chars[0].State);
      Assert.NotEqual(0, chars[1].State);
      Assert.NotEqual(0, chars[2].State);
    }
  }
}
