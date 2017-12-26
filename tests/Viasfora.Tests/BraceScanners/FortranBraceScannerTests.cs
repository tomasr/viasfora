using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Xunit;

namespace Viasfora.Tests.BraceScanners {
  public class FortranBraceScannerTests : BaseScannerTests {

    [Fact]
    public void CanExtractParens() {
      String input = @"(x*(y+7))";
      var extractor = new FortranBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(4, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInSingleLineComment() {
      String input = @"
call F(1);
! call F(2);
";
      var extractor = new FortranBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInString_SingleQuoted() {
      String input = "call F('some (string)')";
      var extractor = new FortranBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInString_SingleQuoted_Quoted() {
      String input = "call F('some''ss (string)')";
      var extractor = new FortranBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }

    [Fact]
    public void IgnoreBracesInString_DoubleQuoted_Quoted() {
      String input = "call F(\"some\"\"ss (string)\")";
      var extractor = new FortranBraceScanner();
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
  }
}
