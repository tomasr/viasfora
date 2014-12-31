using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.BraceExtractors {
  public class PsBraceExtractorTests {
    [Fact]
    public void SimpleFunction() {
      String input = @"
function to-hex([long] $dec) {
   return '0x' + $dec.ToString('X')
}";
      var extractor = new PsBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(8, chars.Count);
    }
    
    [Fact]
    public void BracesInSingleLineCommentsAreIgnored() {
      String input = @"
# some {}[]() braces here
";
      var extractor = new PsBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInMultiLineCommentsAreIgnored() {
      String input = @"
<#
# some {}[]() braces here
#>
";
      var extractor = new PsBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInSingleQuotedStringAreIgnored() {
      String input = @"
'some {}[]() braces here'
";
      var extractor = new PsBraceExtractor();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInDoubleQuotedStringAreIgnored() {
      String input = @"
""some {}[]() braces here""
";
      var extractor = new PsBraceExtractor();
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
