using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Xunit;
using Winterdom.Viasfora.Languages.Sequences;

namespace Viasfora.Tests.Sequences {
  public class FSharpEscapeSequenceParserTests {
    [Fact]
    public void TripleQuoteMeansIgnoreSequences() {
      String input = "\"\"\"some string\\escape\"\"\"";
      var parser = new FSharpEscapeSequenceParser(input);
      Assert.Equal(null, parser.Next());
    }
  }
}
