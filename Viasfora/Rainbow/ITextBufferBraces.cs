using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Rainbow {
  public interface ITextBufferBraces {
    ITextSnapshot Snapshot { get; }
    String BraceChars { get; }
    int LastParsedPosition { get; }
    bool Enabled { get; }

    void Invalidate(SnapshotPoint startPoint);
    void UpdateSnapshot(ITextSnapshot snapshot);
    IEnumerable<BracePos> BracesInSpans(NormalizedSnapshotSpanCollection spans);

    IEnumerable<BracePos> BracesFromPosition(int position);
    Tuple<BracePos, BracePos> GetBracePair(SnapshotPoint point);
    Tuple<BracePos, BracePos> GetBracePairFromPosition(SnapshotPoint point, RainbowHighlightMode mode);
  }
}
