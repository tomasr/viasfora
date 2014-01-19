using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class UserOutliningTagger : ITagger<IOutliningRegionTag>, IUserOutlining {
    private ITextBuffer theBuffer;
    private BufferOutlines regions;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public UserOutliningTagger(ITextBuffer buffer) {
      this.theBuffer = buffer;
      this.regions = new BufferOutlines();
    }

    public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      var snapshot = theBuffer.CurrentSnapshot;
      foreach ( var trackingSpan in regions.Enumerate() ) {
        var spSpan = trackingSpan.GetSpan(snapshot);
        if ( spans.IntersectsWith(new NormalizedSnapshotSpanCollection(spSpan)) ) {
          yield return new TagSpan<IOutliningRegionTag>(spSpan,
            new OutliningRegionTag(false, false, "...", spSpan.GetText()));
        }
      }
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
  }
}
