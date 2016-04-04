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
    private IClassificationType rainbowError;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    public RainbowColorTagger(RainbowProvider provider) {
      this.provider = provider;
      this.rainbowTags = GetRainbows(provider.Registry, Rainbows.MaxDepth);
      this.rainbowError = provider.Registry.GetClassificationType(Rainbows.RainbowError);
    }

    public static IClassificationType[] GetRainbows(IClassificationTypeRegistryService registry, int max) {
      var result = new IClassificationType[max];
      for ( int i = 0; i < max; i++ ) {
        result[i] = registry.GetClassificationType(Rainbows.Rainbow + (i + 1));
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

      var braceCache = provider.BufferBraces;
      ITextSnapshot snapshot = spans[0].Snapshot;
      if ( braceCache == null || !braceCache.Enabled ) {
        yield break;
      }
      if ( braceCache.Snapshot != spans[0].Snapshot ) {
        yield break;
      }

      foreach ( var brace in braceCache.BracesInSpans(spans) ) {
        var ctype = rainbowTags[brace.Depth % this.provider.Settings.RainbowDepth];
        yield return brace.ToSpan(snapshot, ctype);
      }
      foreach ( var error in braceCache.ErrorBracesInSpans(spans) ) {
        yield return new TagSpan<RainbowTag>(
          new SnapshotSpan(snapshot, error.Position, 1),
          new RainbowTag(rainbowError)
          );
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
