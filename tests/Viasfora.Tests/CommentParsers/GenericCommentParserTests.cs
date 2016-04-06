using System;
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
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void SqlBasedSingleLineComment() {
      const String input = "-- vim: ts=4:sw=8";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void VbBasedSingleLineComment() {
      const String input = "' vim: ts=4:sw=8";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void PythonBasedSingleLineComment() {
      const String input = "# vim: ts=4:sw=8";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }
    
    [Fact]
    public void CBasedMultiLineComment() {
      const String input = "/* vim: ts=4:sw=8 */";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void CBasedMultiLineWithoutTerminatorComment() {
      const String input = "/* vim: ts=4:sw=8     ";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void FSharpMultiLineComment() {
      const String input = "(* vim: ts=4:sw=8 *)";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void XmlComment() {
      const String input = "<!-- vim: ts=4:sw=8 -->";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }

    [Fact]
    public void ParsesLeadingWhiteSpace() {
      const String input = "\t   // vim: ts=4:sw=8";
      var parser = new GenericCommentParser();
      String result = parser.Parse(new StringChars(input));
      Assert.Equal("vim: ts=4:sw=8", result);
    }
  }
}
