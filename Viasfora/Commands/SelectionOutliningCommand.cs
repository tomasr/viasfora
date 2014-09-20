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
  public class SelectionOutliningCommand : VsCommand {
    public SelectionOutliningCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidSelectionOutlining);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      if ( TextEditor.GetCurrentView() != null ) {
        if ( HasFeatureOutlines() ) {
          Command.Text = "Remove Selection Outline";
          Command.Enabled = true;
          return;
        } else if ( TextEditor.GetCurrentSelection() != null ) {
          Command.Text = "Outline Selection";
          Command.Enabled = true;
          return;
        }
      }
      Command.Enabled = false;
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      if ( HasFeatureOutlines() ) {
        ClearOutlines();
        return;
      }
      ITextSelection selection = TextEditor.GetCurrentSelection();
      if ( selection == null ) {
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
        CollapseOutlines(selection.TextView);
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


    private void AddOutlining(ITextBuffer buffer, SnapshotSpan span) {
      var outlines = SelectionOutliningManager.Get(buffer);
      outlines.Add(span);
    }
    private void ClearOutlines() {
      var view = TextEditor.GetCurrentView();
      var controller = SelectionOutliningController.Get(view);
      controller.RemoveRegions();
    }
    private bool HasFeatureOutlines() {
      var view = TextEditor.GetCurrentView();
      var outlines = SelectionOutliningManager.Get(view.TextBuffer);
      return outlines.HasUserOutlines();
    }
    private void CollapseOutlines(ITextView textView) {
      var controller = SelectionOutliningController.Get(textView);
      controller.CollapseRegions();
    }

  }
}
