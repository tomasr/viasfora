using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Outlining {
  public class BufferOutlines {
    private List<ITrackingSpan> regions = new List<ITrackingSpan>();

    public int Count => regions.Count;

    public void Add(SnapshotSpan span) {
      this.regions.Add(span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive));
    }
    public void Add(ITrackingSpan span) {
      this.regions.Add(span);
    }
    public IEnumerable<ITrackingSpan> Enumerate() {
      return this.regions;
    }
    public void Clear() {
      this.regions.Clear();
    }
    public int FindRegionContaining(SnapshotPoint point) {
      int candidate = -1;
      int distance = Int32.MaxValue;
      for (int i=0; i < this.regions.Count; i++ ) {
        var spSpan = this.regions[i].GetSpan(point.Snapshot);
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
      var trackingSpan = this.regions[index];
      var result = trackingSpan.GetSpan(snapshot);
      this.regions.RemoveAt(index);
      return result;
    }

    public void LoadStoredData(ITextSnapshot snapshot, OutlineSettings settings) {
      foreach ( var region in settings.Regions ) {
        int start = region.Item1;
        int len = region.Item2;
        if ( start >= snapshot.Length || (start+len) > snapshot.Length ) {
          continue;
        }
        SnapshotSpan span = new SnapshotSpan(snapshot, start, len);
        this.Add(span);
      }
    }
    public OutlineSettings GetStorableData(ITextSnapshot snapshot) {
      OutlineSettings settings = new OutlineSettings();
      foreach ( var trackingSpan in this.regions ) {
        SnapshotSpan span = trackingSpan.GetSpan(snapshot);
        if ( !span.IsEmpty ) {
          settings.Regions.Add(new Tuple<int, int>(span.Start, span.Length));
        }
      }
      return settings;
    }
  }
}
