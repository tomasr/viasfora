using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public interface IUserOutlining {
    void Add(SnapshotSpan span);
    void RemoveAt(SnapshotPoint point);
    bool IsInOutliningRegion(SnapshotPoint point);
  }
}
