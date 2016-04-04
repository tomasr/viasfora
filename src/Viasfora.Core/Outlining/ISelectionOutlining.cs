using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Outlining {
  public interface ISelectionOutlining : IUserOutlining {
    void CreateRegionsAround(SnapshotSpan selectionSpan);
  }
}
