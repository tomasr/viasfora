using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Text {
  public class BraceCache {

    private List<BracePos> braces = new List<BracePos>();
    private List<int> lineCache = new List<int>();
    public ITextSnapshot Snapshot { get; private set; }

    public BraceCache(ITextSnapshot snapshot) {
      this.Snapshot = snapshot;
      EnsureLineCacheCapacity(snapshot.LineCount);
    }

    public void Add(BracePos brace) {
      // if this brace is on a new line
      // store its position in the line cache
      int thisLineNum = Snapshot.GetLineNumberFromPosition(brace.Position);
      if ( braces.Count > 0 ) {
        int lastPosition = braces[braces.Count - 1].Position;
        int lastLineNum = Snapshot.GetLineNumberFromPosition(lastPosition);
        if ( lastLineNum != thisLineNum ) {
          lineCache[thisLineNum] = braces.Count;
        }
      } else {
        lineCache[thisLineNum] = braces.Count;
      }
      braces.Add(brace);
    }

    public IEnumerable<BracePos> BracesInSpans(NormalizedSnapshotSpanCollection spans) {
      foreach ( var wantedSpan in spans ) {
        int startIndex = FindIndexOfFirstBraceInSpan(wantedSpan);
        if ( startIndex < 0 ) {
          continue;
        }
        for ( int j = startIndex; j < braces.Count; j++ ) {
          BracePos bp = braces[j];
          if ( bp.Position >= wantedSpan.End ) break;
          yield return bp;
        }
      }
    }

    public IEnumerable<BracePos> AllBraces() {
      return braces;
    }

    private int FindIndexOfFirstBraceInSpan(SnapshotSpan wantedSpan) {
      int line = Snapshot.GetLineNumberFromPosition(wantedSpan.Start);
      int spanEndLine = Snapshot.GetLineNumberFromPosition(wantedSpan.End);
      for ( ; line < spanEndLine; line++ ) {
        // line contains at least one brace, return it's
        // index within the braces list
        if ( lineCache[line] >= 0 ) {
          return lineCache[line];
        }
      }
      // no braces within the expected span
      return -1;
    }

    private void EnsureLineCacheCapacity(int capacity) {
      lineCache.Capacity = capacity;
      for ( int i = 0; i < capacity; i++ ) {
        lineCache.Add(-1);
      }
    }
  }
}
