using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Contracts {
  public interface IRoslynLanguage {
    IBraceExtractor NewBraceExtractor(ITextBuffer buffer);
  }
}
