using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Outlining;

namespace Winterdom.Viasfora.Commands {
  public class RemoveOutliningCommand : VsCommand {
    public RemoveOutliningCommand(VsfPackage package, OleMenuCommandService omcs) 
      : base(package, omcs) {

      Initialize(new Guid(Guids.guidVsfTextEditorCmdSet), PkgCmdIdList.cmdidRemoveOutlining);
    }

    protected override void OnBeforeQueryStatus(object sender, EventArgs e) {
      base.OnBeforeQueryStatus(sender, e);
      ITextView view = TextEditor.GetCurrentView();
      if ( view == null ) return;
      ITextCaret caret = view.Caret;

      if ( caret == null ) return;
      var point = TextEditor.MapCaretToPrimaryBuffer(view);
      //SnapshotPoint? point = caret.Position.BufferPosition;

      if ( point != null ) {
        IUserOutlining outlining = 
          UserOutliningManager.Get(point.Value.Snapshot.TextBuffer);
        Command.Enabled = outlining.IsInOutliningRegion(point.Value);
      }
    }
    protected override void OnInvoke(object sender, EventArgs e) {
      base.OnInvoke(sender, e);
      ITextView view = TextEditor.GetCurrentView();
      if ( view == null ) return;
      ITextCaret caret = view.Caret;

      if ( caret == null ) return;

      var point = TextEditor.MapCaretToPrimaryBuffer(view);
      //SnapshotPoint? point = caret.Position.BufferPosition;
      if ( point != null ) {
        IUserOutlining outlining = 
          UserOutliningManager.Get(point.Value.Snapshot.TextBuffer);
        outlining.RemoveAt(point.Value);
        Telemetry.WriteEvent("Remove Outlining");
      }
    }
  }
}
