using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {
  public class BraceCache {
    private List<BracePos> braces = new List<BracePos>();
    private List<int> lineCache = new List<int>();
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    public ITextSnapshot Snapshot { get; private set; }
    public int LastParsedPosition { get; private set; }
    public LanguageInfo Language { get; private set; }
    private IBraceExtractor braceExtractor;

    public BraceCache(ITextSnapshot snapshot, IContentType contentType) {
      this.Snapshot = snapshot;
      this.LastParsedPosition = -1;
      this.Language = VsfPackage.LookupLanguage(contentType);
      if ( this.Language != null ) {
        this.braceExtractor = this.Language.NewBraceExtractor();

        this.braceList.Clear();
        String braceChars = Language.BraceList;
        for ( int i = 0; i < braceChars.Length; i += 2 ) {
          this.braceList.Add(braceChars[i], braceChars[i + 1]);
        }
        EnsureLineCacheCapacity(snapshot.LineCount, 0);
      }
    }


    public void Invalidate(SnapshotPoint startPoint) {
      if ( this.Language == null ) return;
      // the new start belongs to a different snapshot!
      var newSnapshot = startPoint.Snapshot;
      this.Snapshot = newSnapshot;

      var end = new SnapshotPoint(newSnapshot, newSnapshot.Length);
      var span = new SnapshotSpan(startPoint, end);
      // remove everything cached after the startPoint
      int index = FindIndexOfFirstBraceInSpan(span);
      if ( index >= 0 ) {
        InvalidateFromBraceAtIndex(newSnapshot, index);
      } else {
        // otherwise, there are no braces after startPoint
        int startLine = newSnapshot.GetLineNumberFromPosition(startPoint.Position);
        EnsureLineCacheCapacity(this.Snapshot.LineCount, startLine);
      }
      //ContinueParsing(startPoint, Snapshot.Length);
    }

    public IEnumerable<BracePos> BracesInSpans(NormalizedSnapshotSpanCollection spans) {
      if ( this.Language == null ) yield break;
      foreach ( var wantedSpan in spans ) {
        EnsureLinesInPreferredSpan(wantedSpan);
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

    public IEnumerable<BracePos> BracesFromPosition(int position) {
      if ( this.Language == null ) return new BracePos[0];
      SnapshotSpan span = new SnapshotSpan(Snapshot, position, Snapshot.Length - position);
      return BracesInSpans(new NormalizedSnapshotSpanCollection(span));
    }

    // We don't want to parse the document in small spans
    // as it is to expensive, so force a larger span if
    // necessary
    private void EnsureLinesInPreferredSpan(SnapshotSpan span) {
      const int MIN_SPAN_LEN = 100;
      var realSpan = span;
      if ( span.Length < MIN_SPAN_LEN ) {
        int end = Math.Min(span.Snapshot.Length, span.Start.Position + MIN_SPAN_LEN);
        realSpan = new SnapshotSpan(span.Start, end - span.Start);
      }
      EnsureLinesInSpan(realSpan);
    }

    private void EnsureLinesInSpan(SnapshotSpan span) {
      if ( span.End > this.LastParsedPosition ) {
        ExtractUntil(span.End);
      }
    }

    private void ExtractUntil(int position) {
      ContinueParsing(this.LastParsedPosition, position);
    }

    private void ContinueParsing(int lastPointParsed, int parseUntil) {
      int startPosition = 0;
      int lastGoodBrace = 0;
      // figure out where to start parsing again
      Stack<BracePos> pairs = new Stack<BracePos>();
      for ( ; lastGoodBrace < braces.Count; lastGoodBrace++ ) {
        BracePos r = braces[lastGoodBrace];
        if ( r.Position >= lastPointParsed ) break;
        if ( IsOpeningBrace(r.Brace) ) {
          pairs.Push(r);
        } else if ( pairs.Count > 0 ) {
          pairs.Pop();
        }
        startPosition = r.Position + 1;
      }
      if ( lastGoodBrace < braces.Count ) {
        braces.RemoveRange(lastGoodBrace, braces.Count - lastGoodBrace);
      }

      ExtractBraces(pairs, startPosition, parseUntil);
    }

    private void ExtractBraces(Stack<BracePos> pairs, int startOffset, int endOffset) {
      int lineNum = Snapshot.GetLineNumberFromPosition(startOffset);
      while ( lineNum < Snapshot.LineCount  ) {
        var line = Snapshot.GetLineFromLineNumber(lineNum++);
        var lineOffset = startOffset > 0 ? startOffset - line.Start : 0;
        ExtractFromLine(pairs, line, lineOffset);
        startOffset = 0;
        if ( line.End >= endOffset ) break;
      }
    }

    private void ExtractFromLine(Stack<BracePos> pairs, ITextSnapshotLine line, int lineOffset) {
      var lc = new LineChars(line, lineOffset);
      var bracesInLine = this.braceExtractor.Extract(lc) /*.ToArray() */;
      foreach ( var cp in bracesInLine ) {
        if ( IsOpeningBrace(cp) ) {
          BracePos p = cp.AsBrace(line.LineNumber, pairs.Count);
          pairs.Push(p);
          Add(p);
        } else if ( IsClosingBrace(cp) && pairs.Count > 0 ) {
          BracePos p = pairs.Peek();
          if ( braceList[p.Brace] == cp.Char ) {
            // yield closing brace
            pairs.Pop();
            BracePos c = cp.AsBrace(line.LineNumber, p.Depth);
            Add(c);
          }
        }
      }
      this.LastParsedPosition = line.End;
    }

    private void Add(BracePos brace) {
      // if this brace is on a new line
      // store its position in the line cache
      int thisLineNum = brace.LineNumber;
      if ( braces.Count > 0 ) {
        int lastLineNum = braces[braces.Count - 1].LineNumber;
        if ( lastLineNum != thisLineNum ) {
          lineCache[thisLineNum] = braces.Count;
        }
      } else {
        lineCache[thisLineNum] = braces.Count;
      }
      braces.Add(brace);
      LastParsedPosition = brace.Position;
    }

    private int FindIndexOfFirstBraceInSpan(SnapshotSpan wantedSpan) {
      int line = Snapshot.GetLineNumberFromPosition(wantedSpan.Start);
      int spanEndLine = Snapshot.GetLineNumberFromPosition(wantedSpan.End);
      for ( ; line <= spanEndLine; line++ ) {
        // line contains at least one brace, return it's
        // index within the braces list
        if ( lineCache[line] >= 0 ) {
          return lineCache[line];
        }
      }
      // no braces within the expected span
      return -1;
    }

    private void EnsureLineCacheCapacity(int capacity, int lastKnownLine) {
      if ( lineCache.Count > capacity ) {
        lineCache.RemoveRange(capacity, lineCache.Count - capacity);
      }
      lineCache.Capacity = capacity;
      for ( int i = lastKnownLine; i < capacity; i++ ) {
        lineCache.Add(-1);
      }
    }

    private void InvalidateFromBraceAtIndex(ITextSnapshot snapshot, int index) {
      if ( index >= braces.Count ) {
        // invalidating from the last one, so not much else to do
        return;
      }
      BracePos lastBrace = braces[index];
      // invalidate the brace list
      braces.RemoveRange(index, braces.Count - index);
      int line = 0;
      if ( lastBrace.Position > snapshot.Length ) {
        line = snapshot.LineCount - 1;
      } else {
        line = snapshot.GetLineNumberFromPosition(lastBrace.Position);
      }

      // invalidate the line cache
      // notice we can only clear the current line entry
      // if the brace being invalidated from is actually
      // the first in that line
      if ( lineCache[line] == index ) {
        lineCache[line] = -1;
      }
      EnsureLineCacheCapacity(snapshot.LineCount, line+1);
      // lastBrace isn't on our cache anymore
      if ( braces.Count > 0 ) {
        this.LastParsedPosition = braces[braces.Count - 1].Position;
      } else {
        this.LastParsedPosition = -1;
      }
    }

    private bool IsClosingBrace(char ch) {
      return braceList.Values.Contains(ch);
    }

    private bool IsOpeningBrace(char ch) {
      return braceList.ContainsKey(ch);
    }
  }
}
