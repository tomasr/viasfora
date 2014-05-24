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
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void ComplexRule() {
      String input = @"
.btn-group > .btn + .dropdown-toggle {
  padding-right: 8px;
  padding-left: 8px;
}
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2, chars.Count);
    }
    [Fact]
    public void BracesInCommentsAreIgnored() {
      String input = @"
/* this is a comment with (){}[] braces */
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInMultiLineCommentsAreIgnored() {
      String input = @"
/* this is a comment
with (){}[]
braces
*/
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInSingleQuotedStringsAreIgnored() {
      String input = @"
'../fonts/glyphicons-halflings-regular.eot (){}'
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInDoubleQuotedStringsAreIgnored() {
      String input = @"
""../fonts/glyphicons-halflings-regular.eot (){}""
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void CanHandleEscapeSequencesInSingleQuotedStrings() {
      String input = @"
'\123eab../fonts/glyphicons-halflings-regular.eot\'(){}\''
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void CanHandleEscapeSequencesInDoubleQuotedStrings() {
      String input = @"
""\123eab../fonts/glyphicons-halflings-regular.eot\'(){}\'""
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void BracesInMultilineStringsAreIgnored() {
      String input = @"
'../fonts/glyphicons-halflings-regular.eot \
some other string (){} \
with more stuff'
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(0, chars.Count);
    }
    [Fact]
    public void CompleteComplexRule1() {
      String input = @"
@font-face {
  font-family: 'Glyphicons Halflings';
  src: url('../fonts/glyphicons-halflings-regular.eot');
  src: url('../fonts/glyphicons-halflings-regular.eot?#iefix') format('embedded-opentype'), url('../fonts/glyphicons-halflings-regular.woff') format('woff'), url('../fonts/glyphicons-halflings-regular.ttf') format('truetype'), url('../fonts/glyphicons-halflings-regular.svg#glyphicons-halflingsregular') format('svg');
}
";
      var extractor = new CssBraceExtractor(new Css());
      var chars = Extract(extractor, input.Trim(), 0, 0);
      Assert.Equal(2+2+8*2, chars.Count);
    }

    private IList<CharPos> Extract(IBraceExtractor extractor, string input, int start, int state) {
      extractor.Reset();
      ITextChars chars = new StringChars(input, start);
      return extractor.Extract(chars).ToList();
    }
  }
}
