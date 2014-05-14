using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IVsTextViewCreationListener))]
  [Name("viasfora.text.completion.handler")]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
  public class PlainTextCompletionHandlerProvider : IVsTextViewCreationListener {
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
        () => new PlainTextCompletionCommandHandler(this, textViewAdapter, textView)
        );
    }
  }

  public class PlainTextCompletionCommandHandler : IOleCommandTarget {
    private PlainTextCompletionHandlerProvider provider;
    private IOleCommandTarget nextCommandHandler;
    private ITextView textView;
    private ICompletionSession session;
    private bool sessionStartedByCommand;

    public PlainTextCompletionCommandHandler(
          PlainTextCompletionHandlerProvider provider,
          IVsTextView viewAdapter,
          ITextView textView) {
      this.provider = provider;
      this.textView = textView;
      //add the command to the command chain
      viewAdapter.AddCommandFilter(this, out nextCommandHandler);
    }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
      if ( pguidCmdGroup == Guids.VsfTextEditorCmdSet ) {
        var cmdId = (int)prgCmds[0].cmdID;
        switch ( cmdId) {
          case PkgCmdIdList.cmdidCompleteWord:
            prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
            return VSConstants.S_OK;
        }
      }
      return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
      if ( !VsfSettings.TextCompletionEnabled ) {
        return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }
      if ( VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider) ) {
        return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }

      // preprocess command
      bool handled = false;
      bool filter = false;
      int hr = 0;
      if ( pguidCmdGroup == VSConstants.VSStd2K ) {
        var cmd = (VSConstants.VSStd2KCmdID)nCmdID;
        switch ( cmd ) {
          case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
          case VSConstants.VSStd2KCmdID.COMPLETEWORD:
            // we don't want to take over the existing handler
            // by the language provider, so only handle these
            // commands if there isn't already an Intellisense
            // provider
            if ( !ReSharper.Installed && !NextHandlerHandlesCommand(pguidCmdGroup, nCmdID) ) {
              handled = this.StartSession(true);
            } 
            break;
          case VSConstants.VSStd2KCmdID.RETURN:
            handled = CompleteWord(false);
            break;
          case VSConstants.VSStd2KCmdID.TAB:
            handled = CompleteWord(true);
            break;
          case VSConstants.VSStd2KCmdID.BACKSPACE:
            filter = true;
            break;
          case VSConstants.VSStd2KCmdID.CANCEL:
            handled = CancelSession();
            break;
          case VSConstants.VSStd2KCmdID.TYPECHAR:
            char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            // we *do* want to commit the char, so leave unhandled
            HandleChar(typedChar);
            // we *do* want to filter the result again
            filter = true;
            break;
        }
      } else if ( pguidCmdGroup == Guids.VsfTextEditorCmdSet ) {
        switch ( (int)nCmdID ) {
          case PkgCmdIdList.cmdidCompleteWord:
            handled = StartSession(true);
            break;
        }
      }

      if ( !handled ) {
        // let other commands handle it
        hr = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        if ( filter )
          Filter();
      }

      return hr;
    }

    private bool NextHandlerHandlesCommand(Guid pguidCmdGroup, uint nCmdID) {
      OLECMD[] cmds = new OLECMD[1];
      cmds[0].cmdID = nCmdID;
      int hr = nextCommandHandler.QueryStatus(ref pguidCmdGroup, 1, cmds, IntPtr.Zero);
      if ( ErrorHandler.Failed(hr) )
        return false;

      uint expected = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
      return (cmds[0].cmdf & expected) == expected;
    }

    private void Filter() {
      if ( session != null && !session.IsDismissed ) {
        session.Filter();
      }       
    }

    private bool StartSession(bool byCommand) {
      // do not start a session if there's already another
      // provider that has started one
      var active = provider.CompletionBroker.GetSessions(textView)
                           .Any(s => !s.IsDismissed);
      if ( active )
        return false;
      // already have an active session, so continue
      if ( session != null && !session.IsDismissed )
        return false;
      this.sessionStartedByCommand = byCommand;
      this.TriggerCompletion();
      return true;
    }

    private bool CancelSession() {
      // if we handled the command, still let it
      // be handled by the next handler.
      // This is useful with VsVim, so that pressing esc
      // to cancel completion can drop you back to
      // normal mode
      if ( session != null ) {
        session.Dismiss();
        return sessionStartedByCommand;
      }
      return false;
    }

    private bool CompleteWord(bool force) {
      if ( session == null || session.IsDismissed )
        return false;

      if ( force || session.SelectedCompletionSet.SelectionStatus.IsSelected ) {
        session.Commit();
        return true;
      }
      session.Dismiss();
      return false;
    }

    private bool HandleChar(char typedChar) {
      if ( session != null && !session.IsDismissed ) {
        if ( char.IsWhiteSpace(typedChar) ) {
          CancelSession();
        //} else if ( !IsIdentifierChar(typedChar) ) {
        //  return CompleteWord(false);
        }
        return false;
      }
      if ( !ReSharper.Installed ) {
        return StartSession(false);
      }
      return false;
    }

    private bool IsIdentifierChar(char typedChar) {
      return Char.IsLetterOrDigit(typedChar)
          || typedChar == '$'
          || typedChar == '_'
          || typedChar == '-';
    }

    private bool IsTabOrEnter(uint nCmdID) {
      return nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
          || nCmdID != (uint)VSConstants.VSStd2KCmdID.TAB;
    }
    private bool TriggerCompletion() {
      if ( session != null ) {
        session.Dismiss();
      }
      // locate trigger point
      var caretPoint = textView.Caret.Position.BufferPosition;
      var snapshot = caretPoint.Snapshot;

      var triggerPoint = snapshot.CreateTrackingPoint(
                            caretPoint.Position, 
                            PointTrackingMode.Positive);
      session = provider.CompletionBroker.CreateCompletionSession(
                            this.textView, triggerPoint, true);
      if ( session != null ) {
        session.Dismissed += this.OnSessionDismissed;
        PlainTextCompletionContext.Add(session);
        session.Start();
        return true;
      }
      return false;
    }

    private void OnSessionDismissed(object sender, EventArgs e) {
      session.Dismissed -= this.OnSessionDismissed;
      session = null;
    }
  }
}
