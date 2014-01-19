using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class BufferRegions {
    private List<ITrackingSpan> regions = new List<ITrackingSpan>();

    public void Add(ITrackingSpan span) {
      regions.Add(span);
    }
    public IEnumerable<ITrackingSpan> Enumerate() {
      return regions;
    }
  }
}
