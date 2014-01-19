using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Text;

namespace Winterdom.Viasfora.Commands {
  public class AddOutliningCommand : VsCommand {
    public AddOutliningCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidAddOutlining);
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
        SnapshotSpan span;
        if ( selection.IsReversed ) {
          span = new SnapshotSpan(selection.End.Position, selection.Start.Position);
        } else {
          span = new SnapshotSpan(selection.Start.Position, selection.End.Position);
        }
        AddOutlining(selection.TextView, span);
      }
    }

    private void AddOutlining(ITextView view, SnapshotSpan span) {
      var outlines = UserOutliningTaggerProvider.Get(view.TextBuffer);
      outlines.Add(span);
    }
  }
}
