using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.BraceExtractors {
  public class CssBraceExtractorTests {
    [Fact]
    public void SimpleRule() {
      String input = @"
audio,
canvas,
video {
  display: inline-block;
}";
      var extractor = new CssBraceExtractor(new PowerShell());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    private IList<CharPos> Extract(IBraceExtractor extractor, string input, int start, int state) {
      extractor.Reset();
      ITextChars chars = new StringChars(input, start);
      return extractor.Extract(chars).ToList();
    }
  }
}
