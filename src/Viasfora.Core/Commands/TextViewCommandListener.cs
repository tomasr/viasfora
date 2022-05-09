using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace Winterdom.Viasfora.Commands {
  [Export(typeof(IVsTextViewCreationListener))]
  [Name("viasfora.command.handler")]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
  public class TextViewCommandListener : IVsTextViewCreationListener {
    [Import]
    internal IVsEditorAdaptersFactoryService AdapterService { get; set; }
    [ImportMany]
    public List<ITextViewCommandHandler> CommandHandlers { get; set; }

    public void VsTextViewCreated(IVsTextView textViewAdapter) {
      ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
      if ( textView == null )
        return;

      textView.Properties.GetOrCreateSingletonProperty(
        () => new TextViewCommandHandler(this, textViewAdapter, textView)
      );
    }

    public ITextViewCommandHandler FindHandler(Guid cmdGroup, int cmdId) {
      // TODO: Optimize this
      foreach ( var cmd in this.CommandHandlers ) {
        if ( cmd.CommandGroup == cmdGroup && cmd.CommandId == cmdId ) {
          return cmd;
        }
      }
      return null;
    }
  }

  public class TextViewCommandHandler : IOleCommandTarget {
    private TextViewCommandListener provider;
    private IOleCommandTarget nextCommandHandler;
    private ITextView textView;

    public TextViewCommandHandler(TextViewCommandListener provider, IVsTextView viewAdapter, ITextView textView) {
      this.provider = provider;
      this.textView = textView;
      //add the command to the command chain
      viewAdapter.AddCommandFilter(this, out this.nextCommandHandler);
    }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
      ThreadHelper.ThrowIfNotOnUIThread();
      var cmdId = (int)prgCmds[0].cmdID;
      var handler = this.provider.FindHandler(pguidCmdGroup, cmdId);
      if ( handler != null ) {
        String commandText = "";
        bool isEnabled = handler.IsEnabled(this.textView, ref commandText);
        if ( isEnabled ) {
          prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
          if ( !String.IsNullOrEmpty(commandText) ) {
            SetOleCmdText(pCmdText, commandText);
          }
          return VSConstants.S_OK;
        }
      }
      return this.nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    public void SetOleCmdText(IntPtr pCmdText, string text) {
      // taken from https://social.msdn.microsoft.com/Forums/vstudio/en-US/e8c9418f-99a8-41d7-bb85-a7db15664abf/how-to-change-menu-controllers-text-dynamically?forum=vsx
      OLECMDTEXT CmdText = (OLECMDTEXT)Marshal.PtrToStructure(pCmdText, typeof(OLECMDTEXT));
      char[] buffer = text.ToCharArray();
      IntPtr pText = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "rgwz"));
      IntPtr pCwActual = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "cwActual"));
      // The max chars we copy is our string, or one less than the buffer size,
      // since we need a null at the end.
      int maxChars = (int)Math.Min(CmdText.cwBuf - 1, buffer.Length);
      Marshal.Copy(buffer, 0, pText, maxChars);
      // append a null
      Marshal.WriteInt16((IntPtr)((long)pText + (long)maxChars * 2), (Int16)0);
      // write out the length + null char
      Marshal.WriteInt32(pCwActual, maxChars + 1);
    }

    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
      ThreadHelper.ThrowIfNotOnUIThread();
      int hr = VSConstants.S_OK;
      var cmdId = (int)nCmdID;
      var handler = this.provider.FindHandler(pguidCmdGroup, cmdId);
      bool handled = false;
      if ( handler != null ) {
        handled = handler.Handle(this.textView);
      }

      if ( !handled ) {
        // let other commands handle it
        hr = this.nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }
      return hr;
    }
  }
}
