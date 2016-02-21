using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class FSharpBraceScannerTests : BaseScannerTests{
    [Fact]
    public void IgnoreBracesInTripleQuotes() {
      String input = "printfn \"\"\"(){}[]\"\"\"";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInTripleQuotes2() {
      String input = "printfn \"\"\"(){}\"[]\"\"\"";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInVerbatimStrings() {
      String input = "print @\"some()\r\ntext()\r\nsome more()\"";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInMultiLineString() {
      String input = "print \"some()\r\ntext()\r\nsome more()\"";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void ConsidersParensStarAsStartOfMultiLineComment() {
      String input = @"
(*
let rainbowOk = ( 3 * (2 + 7 ))
*)";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void ConsidersParensStarParensAsValidExpression() {
      String input = @"
let multiplyOperator_LooksLikeComment = (*)
let rainbowBroken = multiplyOperator_LooksLikeComment 3 (2 + 7)
";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void IgnoreParensInSingleQuotes() {
      String input = @"
let munge (s : string) = s.Replace("" "", """").Replace('(', '.').Replace(')', '.')
";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(8, chars.Count);
    }
    [Fact]
    public void HandleParensInIncompleteSingleQuotesProperly() {
      String input = @"
let munge (s : string) = s.Replace("" "", """").Replace('()
";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(6, chars.Count);
    }
    [Fact]
    public void HandleGenericsCorrectly() {
      String input = @"
let function1 (x: 'a) (y: 'a)
";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void HandleQuoteAtEndOfIdentifier() {
      String input = @"
let c' = 7
let x = (3 + c')
";
      var extractor = new FSharpBraceScanner();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
  }
}
