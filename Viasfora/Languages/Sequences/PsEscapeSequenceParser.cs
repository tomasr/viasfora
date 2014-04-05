using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class PsEscapeSequenceParser : IEscapeSequenceParser {
    private String text;
    private int start;
    public PsEscapeSequenceParser(String text) {
      this.text = text;
      this.start = 0;
    }
    public Span? Next() {
      return null;
    }

  }
}
