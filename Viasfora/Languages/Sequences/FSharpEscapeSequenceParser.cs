using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class FSharpEscapeSequenceParser : CEscapeSequenceParser {
    public FSharpEscapeSequenceParser(String text) : base(text) {
      if ( text.StartsWith("\"\"\"") ) {
        this.SetStart(text.Length);
      }
    }
  }
}
