using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Outlining {
  public abstract class BaseOutliningManager : IUserOutlining, IOutliningManager {
    protected BufferOutlines Regions { get; private set; }
    private static readonly SnapshotSpan[] empty = new SnapshotSpan[0];
    private OutliningTagger outliningTagger;
    private GlyphTagger glyphTagger;

    protected BaseOutliningManager(ITextBuffer buffer) {
      this.Regions = new BufferOutlines();
      this.outliningTagger = new OutliningTagger(this);
      this.glyphTagger = new GlyphTagger(this);
    }


    public ITagger<IOutliningRegionTag> GetOutliningTagger() {
      return this.outliningTagger;
    }

    public ITagger<IGlyphTag> GetGlyphTagger() {
      return this.glyphTagger;
    }

    public IEnumerable<SnapshotSpan> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count > 0 ) {
        var snapshot = spans[0].Snapshot;
        return from trackingSpan in Regions.Enumerate()
               let spSpan = trackingSpan.GetSpan(snapshot)
               where spans.IntersectsWith(new NormalizedSnapshotSpanCollection(spSpan))
               select spSpan;
      }
      return empty;
    }

    public void Add(SnapshotSpan span) {
      Regions.Add(span);
      OnSpanAdded(span);
      RaiseTagsChanged(span);
    }

    public void RemoveAt(SnapshotPoint point) {
      int index = Regions.FindRegionContaining(point);
      if ( index >= 0 ) {
        var span = Regions.RemoveAt(point.Snapshot, index);
        OnRegionRemoved(point);
        RaiseTagsChanged(span);
      }
    }

    public bool IsInOutliningRegion(SnapshotPoint point) {
      return Regions.FindRegionContaining(point) >= 0;
    }

    public bool HasUserOutlines() {
      return Regions.Count > 0;
    }

    public void RemoveAll(ITextSnapshot snapshot) {
      var currentSpans = Regions.Enumerate().ToList();
      Regions.Clear();
      foreach ( var trackingSpan in currentSpans ) {
        var span = trackingSpan.GetSpan(snapshot);
        RaiseTagsChanged(span);
      }

      OnAllRegionsRemoved(snapshot);
    }

    private void RaiseTagsChanged(SnapshotSpan span) {
      this.outliningTagger.RaiseTagsChanged(span);
      this.glyphTagger.RaiseTagsChanged(span);
    }
    
    protected virtual void OnSpanAdded(SnapshotSpan span) {
    }
    protected virtual void OnRegionRemoved(SnapshotPoint point) {
    }
    protected virtual void OnAllRegionsRemoved(ITextSnapshot snapshot) {
    }


    private class OutliningTagger : ITagger<IOutliningRegionTag> {
      public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
      private IUserOutlining manager;

      public OutliningTagger(IUserOutlining manager) {
        this.manager = manager;
      }

      public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
        return from span in manager.GetTags(spans)
               select new TagSpan<IOutliningRegionTag>(span, 
                 new OutliningRegionTag(false, false, "...", span.GetText()));
      }
      public void RaiseTagsChanged(SnapshotSpan span) {
        var temp = this.TagsChanged;
        if ( temp != null ) {
          temp(this, new SnapshotSpanEventArgs(span));
        }
      }
    }

    private class GlyphTagger : ITagger<IGlyphTag> {
      public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
      private IUserOutlining manager;

      public GlyphTagger(IUserOutlining manager) {
        this.manager = manager;
      }

      public IEnumerable<ITagSpan<IGlyphTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
        return from span in manager.GetTags(spans)
               select new TagSpan<IGlyphTag>(span, new OutliningGlyphTag());
      }
      public void RaiseTagsChanged(SnapshotSpan span) {
        var temp = this.TagsChanged;
        if ( temp != null ) {
          temp(this, new SnapshotSpanEventArgs(span));
        }
      }
    }
  }
}
