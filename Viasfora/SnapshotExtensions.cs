using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Winterdom.Viasfora {
  public static class SnapshotExtensions {
    public static SnapshotSpan GetSpan<T>(this IMappingTagSpan<T> tagSpan, ITextSnapshot snapshot) where T : ITag {
      var mappedSpans = tagSpan.Span.GetSpans(snapshot);
      return mappedSpans.Count > 0 
        ? mappedSpans[0] 
        : new SnapshotSpan(snapshot, 0, 0);
    }
    public static SnapshotSpan GetSpan(this ITextSnapshot snapshot) {
      return new SnapshotSpan(snapshot, 0, snapshot.Length);
    }
  }
}
