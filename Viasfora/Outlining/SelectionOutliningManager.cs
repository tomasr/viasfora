using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Outlining {
  public class SelectionOutliningManager : BaseOutliningManager, ISelectionOutlining {
    protected SelectionOutliningManager(ITextBuffer buffer) : base(buffer) {
    }

    public static ISelectionOutlining Get(ITextBuffer buffer) {
      return buffer.Properties.GetOrCreateSingletonProperty(() => {
        return new SelectionOutliningManager(buffer);
      });
    }

    public static IOutliningManager GetManager(ITextBuffer buffer) {
      return Get(buffer) as IOutliningManager;
    }

    public void CreateRegionsAround(SnapshotSpan selectionSpan) {
      SnapshotSpan? beginSpan = CalculateBeginSpan(selectionSpan);
      if ( beginSpan.HasValue ) {
        Add(beginSpan.Value);
      }
      SnapshotSpan? endSpan = CalculateEndSpan(selectionSpan);
      if ( endSpan.HasValue ) {
        Add(endSpan.Value);
      }
    }

    private SnapshotSpan? CalculateBeginSpan(SnapshotSpan span) {
      var snapshot = span.Snapshot;
      int startsOnLine = snapshot.GetLineNumberFromPosition(span.Start);
      if ( startsOnLine > 0 ) {
        var previousLine = snapshot.GetLineFromLineNumber(startsOnLine - 1);
        return new SnapshotSpan(snapshot, 0, previousLine.End);
      }
      return null;
    }

    private SnapshotSpan? CalculateEndSpan(SnapshotSpan span) {
      var snapshot = span.Snapshot;
      int endsOnLine = snapshot.GetLineNumberFromPosition(span.End);
      // it could be that the selection ends right at the start of a line
      // in that case, start the collapse from there
      var endingLine = snapshot.GetLineFromLineNumber(endsOnLine);
      if ( endingLine.Start == span.End ) {
        return new SnapshotSpan(snapshot, endingLine.Start, snapshot.Length - endingLine.Start);
      }

      if ( endsOnLine < snapshot.LineCount - 1 ) {
        var nextLine = snapshot.GetLineFromLineNumber(endsOnLine + 1);
        return new SnapshotSpan(snapshot, nextLine.Start, snapshot.Length - nextLine.Start);
      }
      return null;
    }
  }
}
