using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.BraceExtractors {
  public class FSharpBraceExtractorTests : BaseExtractorTests{
    [Fact]
    public void IgnoreBracesInTripleQuotes() {
      String input = "printfn \"\"\"(){}[]\"\"\"";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInTripleQuotes2() {
      String input = "printfn \"\"\"(){}\"[]\"\"\"";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInVerbatimStrings() {
      String input = "print @\"some()\r\ntext()\r\nsome more()\"";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInMultiLineString() {
      String input = "print \"some()\r\ntext()\r\nsome more()\"";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void ConsidersParensStarAsStartOfMultiLineComment() {
      String input = @"
(*
let rainbowOk = ( 3 * (2 + 7 ))
*)";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void ConsidersParensStarParensAsValidExpression() {
      String input = @"
let multiplyOperator_LooksLikeComment = (*)
let rainbowBroken = multiplyOperator_LooksLikeComment 3 (2 + 7)
";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void IgnoreParensInSingleQuotes() {
      String input = @"
let munge (s : string) = s.Replace("" "", """").Replace('(', '.').Replace(')', '.')
";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(8, chars.Count);
    }
    [Fact]
    public void HandleParensInIncompleteSingleQuotesProperly() {
      String input = @"
let munge (s : string) = s.Replace("" "", """").Replace('()
";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(6, chars.Count);
    }
    [Fact]
    public void HandleGenericsCorrectly() {
      String input = @"
let function1 (x: 'a) (y: 'a)
";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }
    [Fact]
    public void HandleQuoteAtEndOfIdentifier() {
      String input = @"
let c' = 7
let x = (3 + c')
";
      var extractor = new FSharpBraceExtractor();
      var chars = ExtractWithLines(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
  }
}
