using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Rainbow {
  public interface IBraceExtractor {
    String BraceList { get; }
    void Reset();
    IEnumerable<CharPos> Extract(ITextChars text);
  }
}
