using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public class ViewTipProperty {
    public int LineNumber { get; private set; }
    public ViewTipProperty(int line) {
      this.LineNumber = line;
    }
  }
}
