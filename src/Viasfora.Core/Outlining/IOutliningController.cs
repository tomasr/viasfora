using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Outlining {
  public interface IOutliningController {
    void CollapseSelectionRegions();
    void RemoveSelectionRegions();
    void CollapseRegion(SnapshotSpan span);
  }
}
