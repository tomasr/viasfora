using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;

namespace Viasfora.Tests.Sequences {
  // SEE: http://fsharp.org/specs/language-spec/3.0/FSharpSpec-3.0-final.pdf
  public class FSharpEscapeSequenceParserTests {
    [Fact]
    public void SimpleEscapeSequences() {
      // regexp escape-char = '\' ["\'ntbrafv]       String input = "\"\\\"|" +
          @"\\|\'|\n|\t|\b|\r|\a|\f|\v"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(new Span(1,2), parser.Next());
      Assert.Equal(new Span(4,2), parser.Next());
      Assert.Equal(new Span(7,2), parser.Next());
      Assert.Equal(new Span(10,2), parser.Next());
      Assert.Equal(new Span(13,2), parser.Next());
      Assert.Equal(new Span(16,2), parser.Next());
      Assert.Equal(new Span(19,2), parser.Next());
      Assert.Equal(new Span(22,2), parser.Next());
      Assert.Equal(new Span(25,2), parser.Next());
      Assert.Equal(new Span(28,2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NonEscapeSequenceIsNotHighlighted() {
      // regexp non-escape-chars = '\' [^"\'ntbrafv]       String input = "\"" +
          @"\h\l"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TrigraphReturnedAsEscapeSequence() {
      // regexp trigraph = '\' digit-char digit-char digit-char       String input = "\"" +
          @"\023\124"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(new Span(1, 4), parser.Next());
      Assert.Equal(new Span(5, 4), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void NullCharReturnedAsEscapeSequence() {
      // \0
      String input = "\"" +
          @"\0||||\0"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(new Span(1, 2), parser.Next());
      Assert.Equal(new Span(7, 2), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TwoDigitAfterBackslashNotConsideredTrigraph() {
      String input = "\"" +
          @"\02abcd"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UnicodeGraphShortIsEscapeSequence() {
      String input = "\"" +
          @"\u1234some other stuff"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(new Span(1, 6), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UnicodeGraphShortIncompleteIsNotReturned() {
      String input = "\"" +
          @"\u12some other stuff"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void UnicodeGraphLongIsEscapeSequence() {
      String input = "\"" +
          @"\Uabcdef01some other stuff"
          + "\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(new Span(1, 10), parser.Next());
      Assert.Equal(null, parser.Next());
    }
    [Fact]
    public void TripleQuoteMeansIgnoreSequences() {
      String input = "\"\"\"some string\\escape\"\"\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(null, parser.Next());
    }
  }
}
