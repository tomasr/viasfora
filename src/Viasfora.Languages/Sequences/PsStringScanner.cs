using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages.Sequences {
  public class PsStringScanner : IStringScanner {
    private String text;
    private int start;
    public PsStringScanner(String text) {
      this.text = text;
      // quotes are included, so start at 1
      this.start = 1;
      // single-quoted strings in powershell
      // don't support escape sequences
      if ( text.StartsWith("'") || text.StartsWith("@'") )
        this.start = text.Length;
    }
    public StringPart? Next() {
      while ( this.start < this.text.Length - 2 ) {
        if ( this.text[this.start] == '`' ) {
          var span = new TextSpan(this.start, 2);
          this.start += 2;
          return new StringPart(span);
        }
        this.start++;
      }
      return null;
    }

  }
}
