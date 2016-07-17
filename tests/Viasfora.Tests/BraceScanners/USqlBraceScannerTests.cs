using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class USqlBraceScannerTests : BaseScannerTests {

    [Fact]
    public void CanExtractParens() {
      String input = @"
@t =
    EXTRACT date string,
            time string,
            author string,
            tweet string
    FROM ""/Samples/Data/Tweets/MikeDoesBigDataTweets.csv""
    USING Extractors.Csv();
";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    [Fact]
    public void CanExtractBrackets() {
      String input = "ORDER BY [tweet count] DESC";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    [Fact]
    public void CanExtractCurlyBraces() {
      String input = "new SQL.ARRAY<string>{\"nobody\"}";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInSingleLineComments() {
      String input = "// USING Extractors.Csv();";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInMultiLineComments() {
      String input = @"/*
        USING Extractors.Csv();
      */";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInStrings() {
      String input = "\"USING Extractors.Csv()\"";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInChar() {
      String input = "'('";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInCharEscaped() {
      String input = "'\\('";
      var extractor = new USqlBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
  }
}
