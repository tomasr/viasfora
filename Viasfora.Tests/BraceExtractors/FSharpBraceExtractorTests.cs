using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.BraceExtractors {
  public class FSharpBraceExtractorTests {
    [Fact]
    public void IgnoreBracesInTripleQuotes() {
      String input = "printfn \"\"\"(){}[]\"\"\"";
      var extractor = new FSharpBraceExtractor(new FSharp());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInTripleQuotes2() {
      String input = "printfn \"\"\"(){}\"[]\"\"\"";
      var extractor = new FSharpBraceExtractor(new FSharp());
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
