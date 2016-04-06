using Microsoft.VisualStudio.Text;
using System;

namespace Winterdom.Viasfora.Outlining {
  public interface ISelectionOutlining : IUserOutlining {
    void CreateRegionsAround(SnapshotSpan selectionSpan);
  }
}
