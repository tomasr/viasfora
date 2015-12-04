using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringParsers {
  // SEE: http://fsharp.org/specs/language-spec/3.0/FSharpSpec-3.0-final.pdf
  public class FSharpStringParserTests {
    [Fact]
    public void SimpleEscapeSequences() {
      // regexp escape-char = '\' ["\'ntbrafv]       String input = "\"\\\"|" +
          @"\\|\'|\n|\t|\b|\r|\a|\f|\v"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(new StringPart(1,2), parser.Next());
      Assert.Equal(new StringPart(4,2), parser.Next());
      Assert.Equal(new StringPart(7,2), parser.Next());
      Assert.Equal(new StringPart(10,2), parser.Next());
      Assert.Equal(new StringPart(13,2), parser.Next());
      Assert.Equal(new StringPart(16,2), parser.Next());
      Assert.Equal(new StringPart(19,2), parser.Next());
      Assert.Equal(new StringPart(22,2), parser.Next());
      Assert.Equal(new StringPart(25,2), parser.Next());
      Assert.Equal(new StringPart(28,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NonEscapeSequenceIsNotHighlighted() {
      // regexp non-escape-chars = '\' [^"\'ntbrafv]       String input = "\"" +
          @"\h\l"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TrigraphReturnedAsEscapeSequence() {
      // regexp trigraph = '\' digit-char digit-char digit-char       String input = "\"" +
          @"\023\124"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(new StringPart(1, 4), parser.Next());
      Assert.Equal(new StringPart(5, 4), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NullCharReturnedAsEscapeSequence() {
      // \0
      String input = "\"" +
          @"\0||||\0"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(new StringPart(1, 2), parser.Next());
      Assert.Equal(new StringPart(7, 2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoDigitAfterBackslashNotConsideredTrigraph() {
      String input = "\"" +
          @"\02abcd"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UnicodeGraphShortIsEscapeSequence() {
      String input = "\"" +
          @"\u1234some other stuff"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(new StringPart(1, 6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UnicodeGraphShortIncompleteIsNotReturned() {
      String input = "\"" +
          @"\u12some other stuff"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UnicodeGraphLongIsEscapeSequence() {
      String input = "\"" +
          @"\Uabcdef01some other stuff"
          + "\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(new StringPart(1, 10), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TripleQuoteMeansIgnoreSequences() {
      String input = "\"\"\"some string\\escape\"\"\"";
      var parser = new FSharpStringParser(input);
      Assert.Equal(null, parser.Next());
    }
  }
}
