using System;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Viasfora.Tests.StringScanners {
  [Collection("DependsOnVS")]
  // SEE: http://fsharp.org/specs/language-spec/3.0/FSharpSpec-3.0-final.pdf
  public class FSharpStringScannerTests {
    [Fact]
    public void SimpleEscapeSequences() {
      // regexp escape-char = '\' ["\'ntbrafv] 
      String input = "\"\\\"|" +
          @"\\|\'|\n|\t|\b|\r|\a|\f|\v"
          + "\"";
      var parser = new FSharpStringScanner(input);
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
      Assert.Null(parser.Next());
    }
    [Fact]
    public void NonEscapeSequenceIsMarkedAsError() {
      // regexp non-escape-chars = '\' [^"\'ntbrafv] 
      String input = "\"" +
          @"\h\l"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1,2, StringPartType.EscapeSequenceError), parser.Next());
      Assert.Equal(new StringPart(3,2, StringPartType.EscapeSequenceError), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void TrigraphReturnedAsEscapeSequence() {
      // regexp trigraph = '\' digit-char digit-char digit-char 
      String input = "\"" +
          @"\023\124"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1, 4), parser.Next());
      Assert.Equal(new StringPart(5, 4), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void NullCharReturnedAsEscapeSequence() {
      // \0
      String input = "\"" +
          @"\0||||\0"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1, 2), parser.Next());
      Assert.Equal(new StringPart(7, 2), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void TwoDigitAfterBackslashNotConsideredTrigraph() {
      String input = "\"" +
          @"\02abcd"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1, 2, StringPartType.EscapeSequenceError), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void UnicodeGraphShortIsEscapeSequence() {
      String input = "\"" +
          @"\u1234some other stuff"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1, 6), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void UnicodeGraphShortIncompleteIsNotReturned() {
      String input = "\"" +
          @"\u12some other stuff"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1, 3, StringPartType.EscapeSequenceError), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void UnicodeGraphLongIsEscapeSequence() {
      String input = "\"" +
          @"\Uabcdef01some other stuff"
          + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(1, 10), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void TripleQuoteMeansIgnoreSequences() {
      String input = "\"\"\"some string\\nescape\"\"\"";
      var parser = new FSharpStringScanner(input);
      Assert.Null(parser.Next());
    }
    [Fact]
    public void VerbatimMeansIgnoreSequences() {
      String input = "@\"some string\\nescape\"";
      var parser = new FSharpStringScanner(input);
      Assert.Null(parser.Next());
    }


    //
    // Format Specifiers
    //
    [Fact]
    public void SimpleFormatSpecIsExtracted() {
      String input = "\"" + @"some %d value" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(6, 2, StringPartType.FormatSpecifier), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void FormatSpecWithFlagsIsExtracted() {
      String input = "\"" + @"some %+d value" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(6, 3, StringPartType.FormatSpecifier), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void FormatSpecWithFlagsAndWidthIsExtracted() {
      String input = "\"" + @"some %08x value" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(6, 4, StringPartType.FormatSpecifier), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void FormatSpecWithFlagsWidthAndPrecisionIsExtracted() {
      String input = "\"" + @"some %-8.2f value" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(6, 6, StringPartType.FormatSpecifier), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void FormatSpecWithSpaceFlagsIsExtracted() {
      String input = "\"" + @"some % 8x value" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Equal(new StringPart(6, 4, StringPartType.FormatSpecifier), parser.Next());
      Assert.Null(parser.Next());
    }
    [Fact]
    public void DoublePercentIsIgnored() {
      String input = "\"" + @"some %% value" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Null(parser.Next());
    }
    [Fact]
    public void PercentAtEndOfStringIsIgnored() {
      String input = "\"" + @"some %" + "\"";
      var parser = new FSharpStringScanner(input);
      Assert.Null(parser.Next());
    }
  }
}
