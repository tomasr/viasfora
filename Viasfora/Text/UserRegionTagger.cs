using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class UserRegionTagger : ITagger<IOutliningRegionTag> {
    private ITextBuffer theBuffer;
    private BufferRegions regions;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public UserRegionTagger(ITextBuffer buffer) {
      this.theBuffer = buffer;
      this.regions = buffer.Properties.GetOrCreateSingletonProperty(() => {
        return new BufferRegions();
      });
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
  }
}
