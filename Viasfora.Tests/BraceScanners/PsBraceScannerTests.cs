using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class PsBraceScannerTests : BaseScannerTests {
    [Fact]
    public void SimpleFunction() {
      String input = @"
function to-hex([long] $dec) {
   return '0x' + $dec.ToString('X')
}";
      var extractor = new PsBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(8, chars.Count);
    }
    
    [Fact]
    public void BracesInSingleLineCommentsAreIgnored() {
      String input = @"
# some {}[]() braces here
";
      var extractor = new PsBraceScanner();
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
      var extractor = new PsBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInSingleQuotedStringAreIgnored() {
      String input = @"
'some {}[]() braces here'
";
      var extractor = new PsBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInDoubleQuotedStringAreIgnored() {
      String input = @"
""some {}[]() braces here""
";
      var extractor = new PsBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
  }
}
