using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Text {
  public class UserOutliningTagger : ITagger<IOutliningRegionTag>, IUserOutlining, IDisposable {
    private BufferOutlines regions;
    private static readonly TagSpan<IOutliningRegionTag>[] empty =
      new TagSpan<IOutliningRegionTag>[0];

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public UserOutliningTagger(ITextBuffer buffer) {
      this.regions = new BufferOutlines();
      LoadRegions(buffer);
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
      UpdateUserSettings(span.Snapshot.TextBuffer);
      RaiseTagsChanged(span);
    }

    void IUserOutlining.RemoveAt(SnapshotPoint point) {
      int index = regions.FindRegionContaining(point);
      if ( index >= 0 ) {
        var span = regions.RemoveAt(point.Snapshot, index);
        UpdateUserSettings(point.Snapshot.TextBuffer);
        RaiseTagsChanged(span);
      }
    }

    bool IUserOutlining.IsInOutliningRegion(SnapshotPoint point) {
      return regions.FindRegionContaining(point) >= 0;
    }

    bool IUserOutlining.HasUserOutlines() {
      return regions.Count > 0;
    }

    void IUserOutlining.RemoveAll(ITextSnapshot snapshot) {
      var currentSpans = regions.Enumerate().ToList();
      regions.Clear();
      foreach ( var trackingSpan in currentSpans ) {
        var span = trackingSpan.GetSpan(snapshot);
        RaiseTagsChanged(span);
      }
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
      filename = sus.MakeRelativePath(filename);
      OutlineSettings settings = sus.Load<OutlineSettings>(filename);
      if ( settings != null ) {
        this.regions.LoadStoredData(buffer.CurrentSnapshot, settings);
      }
    }
    private void UpdateUserSettings(ITextBuffer buffer) {
      var sus = VsSolution.GetUserSettings();
      if ( sus == null ) {
        return;
      }
      String filename = TextEditor.GetFileName(buffer);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }
      filename = sus.MakeRelativePath(filename);
      sus.Store(filename, regions.GetStorableData(buffer.CurrentSnapshot));
    }
  }
}
