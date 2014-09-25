using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Rainbow {
  class RainbowColorTagger : ITagger<RainbowTag> {
    private RainbowProvider provider;
    private IClassificationType[] rainbowTags;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    public RainbowColorTagger(RainbowProvider provider) {
      this.provider = provider;
      this.rainbowTags = GetRainbows(provider.Registry, Constants.MAX_RAINBOW_DEPTH);
    }
    public static IClassificationType[] GetRainbows(IClassificationTypeRegistryService registry, int max) {
      var result = new IClassificationType[max];
      for ( int i = 0; i < max; i++ ) {
        result[i] = registry.GetClassificationType(Constants.RAINBOW + (i + 1));
      }
      return result;
    }
    public IEnumerable<ITagSpan<RainbowTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      // Needed to prevent race condition on some extensions
      // using a Tag Aggregator while the view is being closed
      if ( provider == null || provider.Settings == null  ) {
        yield break;
      }
      if ( !provider.Settings.RainbowTagsEnabled || spans.Count == 0 ) {
        yield break;
      }
      BraceCache braceCache = provider.BraceCache;
      ITextSnapshot snapshot = spans[0].Snapshot;
      if ( braceCache == null || braceCache.Snapshot != spans[0].Snapshot ) {
        yield break;
      }
      foreach ( var brace in braceCache.BracesInSpans(spans) ) {
        var ctype = rainbowTags[brace.Depth % this.provider.Settings.RainbowDepth];
        yield return brace.ToSpan(snapshot, ctype);
      }
    }
    public void NotifyUpdateTags(SnapshotSpan span) {
      var tempEvent = this.TagsChanged;
      if ( tempEvent != null ) {
        Action action = delegate() {
          tempEvent(this, new SnapshotSpanEventArgs(span));
        };
        provider.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
      }
    }
  }
}
