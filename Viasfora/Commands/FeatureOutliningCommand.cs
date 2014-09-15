using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  public class FeatureOutliningCommand : VsCommand {
    public FeatureOutliningCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidFeatureOutlining);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      Command.Enabled = TextEditor.GetCurrentSelection() != null;
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      ITextSelection selection = TextEditor.GetCurrentSelection();
      if ( selection != null ) {
        if ( selection.Mode == TextSelectionMode.Box ) {
          // not supported, ignore for now;
          return;
        }
        ITextView view = selection.TextView;
        SnapshotSpan? span = TextEditor.MapSelectionToPrimaryBuffer(selection);
        if ( span.HasValue ) {
          SnapshotSpan? beginSpan = CalculateBeginSpan(span.Value);
          if ( beginSpan.HasValue ) {
            AddOutlining(beginSpan.Value.Snapshot.TextBuffer, beginSpan.Value);
          }
          SnapshotSpan? endSpan = CalculateEndSpan(span.Value);
          if ( endSpan.HasValue ) {
            AddOutlining(endSpan.Value.Snapshot.TextBuffer, endSpan.Value);
          }
        }
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
      if ( endsOnLine < snapshot.LineCount - 1 ) {
        var nextLine = snapshot.GetLineFromLineNumber(endsOnLine + 1);
        return new SnapshotSpan(snapshot, nextLine.Start, snapshot.Length - nextLine.Start);
      }
      return null;
    }


    private void AddOutlining(ITextBuffer buffer, SnapshotSpan span) {
      var outlines = FeatureOutliningManager.Get(buffer);
      outlines.Add(span);
    }
  }
}
