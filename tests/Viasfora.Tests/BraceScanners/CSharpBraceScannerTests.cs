using System;
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
    [Fact]
    public void Bug344_InterpolatedStringEmbedded2() {
      String input = "($\"{ $\"{a}\" }\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2+2+2, chars.Count);
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
    public void InterpolatedAtStringStaysVerbatimAfterNestedInterpolatedString() {
      String input = "$@\"{$\"{a}\"}\\{b}\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{{}}{}", Braces(chars));
    }
    [Fact]
    public void InterpolatedStringCanResumeFromBraceInExpression() {
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, "$\"{ a(", 0, 0);
      Assert.Equal("{(", Braces(chars));
      extractor.Reset(chars.Last().State);
      chars = Extract(extractor, "b) }\" (x)", 0, 0, false);
      Assert.Equal(")}()", Braces(chars));
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

    [Fact]
    public void RawString1() {
      String input = "\"\"\"some \r\n string with \r\n{\"quotes\"} \"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void RawStringMultiLineWithQuotesAndBraces() {
      String input = "(\"\"\"\r\n"
                   + "  {\r\n"
                   + "    \"version\": \"0.1\",\r\n"
                   + "    \"issues\": [\r\n"
                   + "      {\r\n"
                   + "  }}}}}}}}\r\n"
                   + "        \"uri\": \"C:\\\\agent\\\\Program.cs\",\r\n"
                   + "        \"shortMessage\": \"It features \\\"quoted text\\\".\",\r\n"
                   + "      }\r\n"
                   + "    ]\r\n"
                   + "  }\r\n"
                   + "  \"\"\")";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input, 0, 0);
      Assert.Equal("()", Braces(chars));
    }
    [Fact]
    public void RawStringSingleLineWithQuotes() {
      String input = "(\"\"\"a \"b\" {c\"\" (\"\"\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("()", Braces(chars));
    }
    [Fact]
    public void RawStringWithLongerDelimiter() {
      String input = "(\"\"\"\"\r\n"
                   + "  \"\"\" { \"\"\"\r\n"
                   + "  \"\"\"\")";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input, 0, 0);
      Assert.Equal("()", Braces(chars));
    }
    [Fact]
    public void RawStringWithLongerDelimiterSingleLine() {
      String input = "(\"\"\"\"a \"\"\" { \"\"\"\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("()", Braces(chars));
    }
    [Fact]
    public void RawString2() {
      String input = "$$\"\"\"class {call()}MyClass\r\n{{ }}\"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal("{}", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringWithQuotes() {
      String input = "($\"\"\"x {a[\"k\"]} \"y\" \"\"\")";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("({[]})", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringMultiLine() {
      String input = "($\"\"\"some \r\n string with \r\n{\"quotes\"} \"\"\")";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input, 0, 0);
      Assert.Equal("({})", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringIgnoresBracesShorterThanDelimiter() {
      String input = "$$\"\"\"\r\n"
                   + "  { \"a\": {{x[0]}} }\r\n"
                   + "  \"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input, 0, 0);
      Assert.Equal("{[]}", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringUsesInnermostBracesForInterpolation() {
      String input = "$$\"\"\"X{{{1+1}}}Z\"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{}", Braces(chars));
      Assert.Equal(new[] { 7, 12 }, chars.Select(c => c.Position));
      Assert.Equal(new[] { 2, 2 }, chars.Select(c => c.Length));
    }
    [Fact]
    public void InterpolatedRawStringDelimitersSpanAllTheirBraces() {
      String input = "$$$\"\"\"{{{a}}}\"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{}", Braces(chars));
      Assert.Equal(new[] { 6, 10 }, chars.Select(c => c.Position));
      Assert.Equal(new[] { 3, 3 }, chars.Select(c => c.Length));
    }
    [Fact]
    public void BracesInInterpolatedRawStringExpressionAreSingleCharacters() {
      String input = "$$\"\"\"{{ a(b) }}\"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{()}", Braces(chars));
      Assert.Equal(new[] { 2, 1, 1, 2 }, chars.Select(c => c.Length));
    }
    [Fact]
    public void InterpolatedRawStringWithBracesInExpression() {
      String input = "$$\"\"\"{{ new[] { 1 }.Length }}\"\"\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{[]{}}", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringWithLongerDelimiter() {
      String input = "$\"\"\"\"a \"\"\" {x} \"\"\"\" (y)";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{}()", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringNestedInInterpolatedString() {
      String input = "$\"{ $$\"\"\"{{a}}\"\"\" }\"";
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, input, 0, 0);
      Assert.Equal("{{}}", Braces(chars));
    }
    [Fact]
    public void InterpolatedRawStringCanResumeFromBraceInExpression() {
      var extractor = new CSharpBraceScanner();
      var chars = Extract(extractor, "$$\"\"\"{{ a(", 0, 0);
      Assert.Equal("{(", Braces(chars));
      extractor.Reset(chars.Last().State);
      chars = Extract(extractor, "b) }} {x} \"\"\" (y)", 0, 0, false);
      Assert.Equal(")}()", Braces(chars));
    }
  }
}
