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
    [Fact]
    public void IgnoreBracesInVerbatimStrings() {
      String input = "print @\"some()\r\ntext()\r\nsome more()\"";
      var extractor = new FSharpBraceExtractor(new FSharp());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void IgnoreBracesInMultiLineString() {
      String input = "print \"some()\r\ntext()\r\nsome more()\"";
      var extractor = new FSharpBraceExtractor(new FSharp());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    private IList<CharPos> Extract(IBraceExtractor extractor, string input, int start, int state) {
      extractor.Reset();

      input = input.Substring(start);

      String[] lines = input.Split('\r', '\n');
      List<CharPos> result = new List<CharPos>();
      foreach ( String line in lines ) {
        ITextChars chars = new StringChars(line);
        while ( !chars.EndOfLine ) {
          result.AddRange(extractor.Extract(chars));
        }
      }
      return result;
    }
  }
}
