using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {
  public class BraceCache {

    private int linesToScan;
    private List<BracePos> braces = new List<BracePos>();
    private List<int> lineCache = new List<int>();
    private Dictionary<char, char> braceList = new Dictionary<char, char>();
    public ITextSnapshot Snapshot { get; private set; }
    public int LastParsedPosition { get; private set; }
    public LanguageInfo Language { get; private set; }

    public BraceCache(ITextSnapshot snapshot, IContentType contentType, int linesToScan) {
      this.Snapshot = snapshot;
      this.linesToScan = linesToScan;
      this.LastParsedPosition = -1;
      this.Language = VsfPackage.LookupLanguage(contentType);

      this.braceList.Clear();
      String braceChars = Language.BraceList;
      for ( int i = 0; i < braceChars.Length; i += 2 ) {
        this.braceList.Add(braceChars[i], braceChars[i + 1]);
      }
      EnsureLineCacheCapacity(snapshot.LineCount);
    }


    public void Invalidate(SnapshotPoint startPoint) {
      var span = new SnapshotSpan(Snapshot, startPoint.Position, 
        Snapshot.Length - startPoint.Position);
      int index = FindIndexOfFirstBraceInSpan(span);
      if ( index >= 0 ) {
        InvalidateFromBraceAtIndex(index);
      }
      this.Snapshot = startPoint.Snapshot;
      EnsureLineCacheCapacity(this.Snapshot.LineCount);
      ContinueParsing(startPoint);
    }

    public IEnumerable<BracePos> BracesInSpans(NormalizedSnapshotSpanCollection spans) {
      foreach ( var wantedSpan in spans ) {
        EnsureLinesInSpan(wantedSpan);
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
      SnapshotSpan span = new SnapshotSpan(Snapshot, position, Snapshot.Length - position);
      return BracesInSpans(new NormalizedSnapshotSpanCollection(span));
    }

    private void EnsureLinesInSpan(SnapshotSpan span) {
      if ( span.End > this.LastParsedPosition ) {
        ExtractUntil(span.End);
      }
    }

    private void ExtractUntil(int position) {
      ContinueParsing(position);
    }

    private void ContinueParsing(int lastPoint) {
      int startPosition = 0;
      int lastGoodBrace = 0;
      // figure out where to start parsing again
      Stack<BracePos> pairs = new Stack<BracePos>();
      for ( ; lastGoodBrace < braces.Count; lastGoodBrace++ ) {
        BracePos r = braces[lastGoodBrace];
        if ( r.Position >= lastPoint ) break;
        if ( IsOpeningBrace(r.Brace) ) {
          pairs.Push(r);
        } else {
          pairs.Pop();
        }
        startPosition = r.Position + 1;
      }
      if ( lastGoodBrace < braces.Count ) {
        braces.RemoveRange(lastGoodBrace, braces.Count - 1);
      }

      BraceExtractor extractor = new BraceExtractor(
        new SnapshotPoint(Snapshot, startPosition), Language);
      foreach ( var pt in extractor.All() ) {
        char ch = pt.GetChar();
        if ( IsOpeningBrace(ch) ) {
          BracePos p = new BracePos {
            Brace = ch, Depth = pairs.Count,
            Position = pt.Position
          };
          pairs.Push(p);
          Add(p);
        } else if ( IsClosingBrace(ch) && pairs.Count > 0 ) {
          BracePos p = pairs.Peek();
          if ( braceList[p.Brace] == ch ) {
            // yield closing brace
            pairs.Pop();
            BracePos c = new BracePos {
              Brace = ch, Depth = p.Depth,
              Position = pt.Position
            };
            Add(c);
          }
        }
      }
    }

    private void Add(BracePos brace) {
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
      LastParsedPosition = brace.Position;
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
      if ( lineCache.Count > capacity ) {
        lineCache.RemoveRange(capacity, lineCache.Count - capacity);
      }
      lineCache.Capacity = capacity;
      for ( int i = lineCache.Count; i < capacity; i++ ) {
        lineCache.Add(-1);
      }
    }

    private void InvalidateFromBraceAtIndex(int index) {
      BracePos lastBrace = braces[index];
      // invalidate the brace list
      braces.RemoveRange(index, braces.Count - index);

      // invalidate the line cache
      int line = Snapshot.GetLineNumberFromPosition(lastBrace.Position);
      if ( lineCache[line] == index ) {
        lineCache[line] = -1;
      }
      for ( ++line; line < lineCache.Count; line++ ) {
        lineCache[line] = -1;
      }
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
