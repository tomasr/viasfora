using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class UserOutliningTagger : ITagger<IOutliningRegionTag>, IUserOutlining, IDisposable {
    private BufferOutlines regions;
    private static readonly TagSpan<IOutliningRegionTag>[] empty =
      new TagSpan<IOutliningRegionTag>[0];

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public UserOutliningTagger() {
      this.regions = new BufferOutlines();
    }

    public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count > 0 )
      {
        var snapshot = spans[0].Snapshot;
        return from trackingSpan in regions.Enumerate()
               let spSpan = trackingSpan.GetSpan(snapshot)
               where spans.IntersectsWith(new NormalizedSnapshotSpanCollection(spSpan))
               select new TagSpan<IOutliningRegionTag>(spSpan, 
                 new OutliningRegionTag(false, false, "...", spSpan.GetText()));
      }
      return empty;
    }

    public void Dispose() {
    }

    private void RaiseTagsChanged(SnapshotSpan span) {
      var temp = this.TagsChanged;
      if ( temp != null ) {
        temp(this, new SnapshotSpanEventArgs(span));
      }
    }


    // user outlining implementation
    void IUserOutlining.Add(SnapshotSpan span) {
      regions.Add(span);
      RaiseTagsChanged(span);
    }

    void IUserOutlining.RemoveAt(SnapshotPoint point) {
      int index = regions.FindRegionContaining(point);
      if ( index >= 0 ) {
        var span = regions.RemoveAt(point.Snapshot, index);
        RaiseTagsChanged(span);
      }
    }

    bool IUserOutlining.IsInOutliningRegion(SnapshotPoint point) {
      return regions.FindRegionContaining(point) >= 0;
    }
  }
}
