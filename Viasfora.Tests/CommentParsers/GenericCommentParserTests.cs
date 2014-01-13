using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Util;
using Xunit;

namespace Viasfora.Tests.CommentParsers {
  public class GenericCommentParserTests {

    [Fact]
    public void CBasedSingleLineComment() {
      const String input = "// vim: ts=4:sw=8";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4;sw=8", result);
    }

  }
}
