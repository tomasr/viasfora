using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IVsTextViewCreationListener))]
  [Name("viasfora.text.completion.handler")]
  [ContentType("plainText")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class AllTextCompletionHandlerProvider : IVsTextViewCreationListener {
    [Import]
    internal IVsEditorAdaptersFactoryService AdapterService { get; set; }
    [Import]
    internal ICompletionBroker CompletionBroker { get; set; }
    [Import]
    internal SVsServiceProvider ServiceProvider { get; set; }
    public void VsTextViewCreated(IVsTextView textViewAdapter) {

      ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
      if ( textView == null )
        return;

      textView.Properties.GetOrCreateSingletonProperty(
        () => new AllTextCompletionCommandHandler(this, textViewAdapter, textView)
        );
    }
  }

  public class AllTextCompletionCommandHandler : IOleCommandTarget {
    private AllTextCompletionHandlerProvider provider;
    private IOleCommandTarget nextCommandHandler;
    private ITextView textView;
    private ICompletionSession session;

    public AllTextCompletionCommandHandler(
          AllTextCompletionHandlerProvider provider,
          IVsTextView viewAdapter,
          ITextView textView) {
      this.provider = provider;
      this.textView = textView;
      //add the command to the command chain
      viewAdapter.AddCommandFilter(this, out nextCommandHandler);
    }
    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
      if ( VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider) ) {
        return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }
      //make a copy of this so we can look at it after forwarding some commands 
      uint commandID = nCmdID;
      char typedChar = char.MinValue;
      //make sure the input is a char before getting it 
      if ( pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR ) {
        typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
      }

      //check for a commit character 
      if ( nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
          || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB
          || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)) ) {
        //check for a a selection 
        if ( session != null && !session.IsDismissed ) {
          //if the selection is fully selected, commit the current session 
          if ( session.SelectedCompletionSet.SelectionStatus.IsSelected ) {
            session.Commit();
            //also, don't add the character to the buffer 
            return VSConstants.S_OK;
          } else {
            //if there is no selection, dismiss the session
            session.Dismiss();
          }
        }
      }
      //pass along the command so the char is added to the buffer 
      int retVal = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      bool handled = false;
      if ( !typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar) ) {
        if ( session == null || session.IsDismissed ) // If there is no active session, bring up completion
        {
          this.TriggerCompletion();
          if ( session != null )
            session.Filter();
        } else
        {
          //the completion session is already active, so just filter
          session.Filter();
        }
        handled = true;
      } else if ( commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE
               || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE ) {
        //redo the filter if there is a deletion
        if ( session != null && !session.IsDismissed )
          session.Filter();
        handled = true;
      }
      if ( handled ) return VSConstants.S_OK;
      return retVal;
    }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
      return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }
    private bool TriggerCompletion() {
      //the caret must be in a non-projection location 
      SnapshotPoint? caretPoint =
      textView.Caret.Position.Point.GetPoint(
      textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
      if ( !caretPoint.HasValue ) {
        return false;
      }

      session = provider.CompletionBroker.CreateCompletionSession(
        textView,
        caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
        true);

      //subscribe to the Dismissed event on the session 
      session.Dismissed += this.OnSessionDismissed;
      session.Start();

      return true;
    }
    private void OnSessionDismissed(object sender, EventArgs e) {
      session.Dismissed -= this.OnSessionDismissed;
      session = null;
    }
  }
}
