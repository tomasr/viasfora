using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Settings;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Outlining {
  public abstract class BaseOutliningManager : IUserOutlining, IOutliningManager {
    private BufferOutlines regions;
    private static readonly SnapshotSpan[] empty = new SnapshotSpan[0];
    private OutliningTagger outliningTagger;
    private GlyphTagger glyphTagger;

    protected BaseOutliningManager(ITextBuffer buffer) {
      this.regions = new BufferOutlines();
      this.outliningTagger = new OutliningTagger(this);
      this.glyphTagger = new GlyphTagger(this);
      LoadRegions(buffer);
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
        return from trackingSpan in regions.Enumerate()
               let spSpan = trackingSpan.GetSpan(snapshot)
               where spans.IntersectsWith(new NormalizedSnapshotSpanCollection(spSpan))
               select spSpan;
      }
      return empty;
    }

    public void Add(SnapshotSpan span) {
      regions.Add(span);
      UpdateUserSettings(span.Snapshot.TextBuffer, span.Snapshot);
      RaiseTagsChanged(span);
    }

    public void RemoveAt(SnapshotPoint point) {
      int index = regions.FindRegionContaining(point);
      if ( index >= 0 ) {
        var span = regions.RemoveAt(point.Snapshot, index);
        UpdateUserSettings(point.Snapshot.TextBuffer, point.Snapshot);
        RaiseTagsChanged(span);
      }
    }

    public bool IsInOutliningRegion(SnapshotPoint point) {
      return regions.FindRegionContaining(point) >= 0;
    }

    public bool HasUserOutlines() {
      return regions.Count > 0;
    }

    public void RemoveAll(ITextSnapshot snapshot) {
      var currentSpans = regions.Enumerate().ToList();
      regions.Clear();
      foreach ( var trackingSpan in currentSpans ) {
        var span = trackingSpan.GetSpan(snapshot);
        RaiseTagsChanged(span);
      }
      UpdateUserSettings(snapshot.TextBuffer, snapshot);
    }

    private void RaiseTagsChanged(SnapshotSpan span) {
      this.outliningTagger.RaiseTagsChanged(span);
      this.glyphTagger.RaiseTagsChanged(span);
    }
    
    private void LoadRegions(ITextBuffer buffer) {
      var sus = VsSolution.GetUserSettings();
      if ( sus == null ) {
        return;
      }
      String filename = TextEditor.GetFileName(buffer);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }
      filename = VsSolution.MakeRelativePath(filename);
      OutlineSettings settings = sus.Load<OutlineSettings>(filename);
      if ( settings != null ) {
        this.regions.LoadStoredData(buffer.CurrentSnapshot, settings);
      }
    }
    private void UpdateUserSettings(ITextBuffer buffer, ITextSnapshot snapshot) {
      var sus = VsSolution.GetUserSettings();
      if ( sus == null ) {
        return;
      }
      String filename = TextEditor.GetFileName(buffer);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }
      filename = VsSolution.MakeRelativePath(filename);
      sus.Store(filename, regions.GetStorableData(snapshot));
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
