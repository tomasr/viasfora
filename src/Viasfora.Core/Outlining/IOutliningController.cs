using Microsoft.VisualStudio.Text;
using System;

namespace Winterdom.Viasfora.Outlining {
  public interface IOutliningController {
    void CollapseSelectionRegions();
    void RemoveSelectionRegions();
    void CollapseRegion(SnapshotSpan span);
  }
}
