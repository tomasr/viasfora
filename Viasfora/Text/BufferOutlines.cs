using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class BufferOutlines {
    private List<ITrackingSpan> regions = new List<ITrackingSpan>();

    public void Add(SnapshotSpan span) {
      regions.Add(span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive));
    }
    public void Add(ITrackingSpan span) {
      regions.Add(span);
    }
    public IEnumerable<ITrackingSpan> Enumerate() {
      return regions;
    }
    public int FindRegionContaining(SnapshotPoint point) {
      int candidate = -1;
      int distance = Int32.MaxValue;
      for (int i=0; i < regions.Count; i++ ) {
        var spSpan = regions[i].GetSpan(point.Snapshot);
        if ( spSpan.Contains(point) ) {
          int d = point - spSpan.Start;
          if ( d < distance ) {
            candidate = i;
          }
        }
      }
      return candidate;
    }
    public SnapshotSpan RemoveAt(ITextSnapshot snapshot, int index) {
      var trackingSpan = regions[index];
      var result = trackingSpan.GetSpan(snapshot);
      regions.RemoveAt(index);
      return result;
    }
  }
}
