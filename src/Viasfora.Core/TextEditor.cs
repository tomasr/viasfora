using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.TextManager.Interop;
using VsOle = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Outlining;
using Winterdom.Viasfora.Compatibility;

namespace Winterdom.Viasfora {
  public static class TextEditor {
    public static ITextCaret GetCurrentCaret() {
      ITextView view = GetCurrentView();
      return view?.Caret;
    }
    public static ITextSelection GetCurrentSelection() {
      ITextView view = GetCurrentView();
      if ( view == null ) {
        return null;
      }
      if ( view.Selection.IsEmpty ) 
        return null;
      return view.Selection;
    }
    public static ITextView GetCurrentView() {
      var textManager = (IVsTextManager)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));

      int hr = textManager.GetActiveView(1, null, out IVsTextView textView);
      if ( hr != Constants.S_OK || textView == null )
        return null;

      var componentModel = new SComponentModel();
      var factory = componentModel.GetService<IVsEditorAdaptersFactoryService>();
      return factory.GetWpfTextView(textView);
    }
    public static bool SupportsOutlines(ITextView view) {
      var componentModel = new SComponentModel();
      var outliningService = componentModel.GetService<IOutliningManagerService>();
      if ( outliningService == null ) {
        return false;
      }
      var outliningManager = outliningService.GetOutliningManager(view);
      return outliningManager != null && outliningManager.Enabled;
    }

    public static String GetFileName(ITextBuffer buffer) {
      if ( buffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer adapter) ) {
        if ( adapter is IPersistFileFormat pff ) {
          String filename;
          try {
            int hr = pff.GetCurFile(out filename, out uint formatIndex);
            // some windows will return E_NOTIMPL
            if ( hr == Constants.E_NOTIMPL )
              return null;
            CheckError(hr, "GetCurFile");
          } catch ( NotImplementedException ) {
            // Lovely stuff: SecondaryVsTextBuffer will
            // fail with a NIE instead of returning E_NOTIMPL O_0
            filename = null;
          }
          return filename;
        }
      }
      return null;
    }

    public static SnapshotSpan? MapSelectionToPrimaryBuffer(ITextSelection selection) {
      var span = selection.StreamSelectionSpan.SnapshotSpan;

      ITextView view = selection.TextView;
      var buffer = GetPrimaryBuffer(view);

      var locations = view.BufferGraph.MapDownToBuffer(
        span, SpanTrackingMode.EdgeInclusive, buffer
      );
      if ( locations.Count > 0 ) {
        span = new SnapshotSpan(locations[0].Start, locations[locations.Count - 1].End);
      }
      return span;
    }

    public static SnapshotPoint? MapCaretToPrimaryBuffer(ITextView view) {
      var buffer = GetPrimaryBuffer(view);
      var caret = view.Caret;
      var point = view.BufferGraph.MapDownToBuffer(
        caret.Position.BufferPosition, 
        PointTrackingMode.Negative, buffer,
        PositionAffinity.Predecessor
      );
      return point;
    }

    public static ITextBuffer GetPrimaryBuffer(ITextView view) {
      var buffers = view.BufferGraph.GetTextBuffers(
          (x) => !x.ContentType.IsOfType("projection")
        );
      if ( buffers.Count <= 0 ) {
        return view.BufferGraph.TopBuffer;
      }
      return buffers[0];
    }

    public static bool IsNonProjectionOrElisionBufferType(Type type) {
      if ( typeof(IProjectionBuffer).IsAssignableFrom(type) )
        return false;
      if ( typeof(IElisionBuffer).IsAssignableFrom(type) )
        return false;
      return true;
    }
    public static bool IsNonProjectionOrElisionBuffer(ITextBuffer buffer) {
      if ( (buffer as IProjectionBuffer) != null )
        return false;
      if ( (buffer as IElisionBuffer) != null )
        return false;
      return true;
    }

    public static void DisplayMessageInStatusBar(string message) {
      IVsStatusbar bar = (IVsStatusbar)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsStatusbar));
      if ( bar != null ) {
        bar.SetText(message);
      }
    }

    //
    // Ugly hack: We write the buffer contents into a
    // temporary file, then open this in the standard
    // text editor as an "external" file and
    // then mark the buffer as read-only
    //
    public static void OpenBufferInPlainTextEditorAsReadOnly(ITextBuffer buffer) {
      OpenBufferInEditorAsReadOnly(buffer, "txt");
    }
    public static void OpenBufferInEditorAsReadOnly(ITextBuffer buffer, String extension) {
      String filepath = SaveBufferToTempPath(buffer, extension);

      var uiShell = (IVsUIShellOpenDocument)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShellOpenDocument));
      var oleSvcProvider = (VsOle.IServiceProvider)
        ServiceProvider.GlobalProvider.GetService(typeof(VsOle.IServiceProvider));

      Guid editorType = VSConstants.VsEditorFactoryGuid.TextEditor_guid;
      Guid logicalView = VSConstants.LOGVIEWID.TextView_guid;
      IVsWindowFrame frame = VsShellUtilities.OpenDocumentWithSpecificEditor(
        ServiceProvider.GlobalProvider,
        filepath, editorType, logicalView
        );
      if ( frame != null ) {
        MarkDocumentAsTemporary(filepath);
        MarkDocumentInFrameAsReadOnly(frame);
        frame.Show();
      }
    }

    private static void MarkDocumentInFrameAsReadOnly(IVsWindowFrame frame) {
      var textView = VsShellUtilities.GetTextView(frame);
      if ( textView.GetBuffer(out IVsTextLines textLines) == Constants.S_OK ) {
        var vsBuffer = textLines as IVsTextBuffer;
        vsBuffer.SetStateFlags((uint)(
          BUFFERSTATEFLAGS.BSF_USER_READONLY |
          BUFFERSTATEFLAGS.BSF_FILESYS_READONLY
        ));
      }
    }

    private static void MarkDocumentAsTemporary(string moniker) {
      IVsRunningDocumentTable docTable = (IVsRunningDocumentTable)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsRunningDocumentTable));

      uint lockType = (uint)_VSRDTFLAGS.RDT_DontAddToMRU
                    | (uint)_VSRDTFLAGS.RDT_NonCreatable
                    | (uint)_VSRDTFLAGS.RDT_VirtualDocument
                    | (uint)_VSRDTFLAGS.RDT_PlaceHolderDoc;

      int hr = docTable.FindAndLockDocument(
        dwRDTLockType: lockType,
        pszMkDocument: moniker,
        ppHier: out IVsHierarchy hierarchy,
        pitemid: out uint itemid,
        ppunkDocData: out IntPtr docData,
        pdwCookie: out uint documentCookie
        );
      CheckError(hr, "FindAndLockDocument");
      docTable.ModifyDocumentFlags(documentCookie, lockType, 1);
    }

    private static string SaveBufferToTempPath(ITextBuffer buffer, String extension) {
      String tempDir = Path.GetTempPath();
      String file = Path.Combine(tempDir, Path.GetRandomFileName());
      file += "." + extension;
      File.WriteAllText(file, buffer.CurrentSnapshot.GetText());
      return file;
    }

    private static void CheckError(int hr, String operation) {
      if ( hr != Constants.S_OK ) {
        var ex = new InvalidOperationException(String.Format("{0} returned 0x{1:x8}", operation, hr));
        PkgSource.LogError(operation, ex);
        throw ex;
      }
    }

  }
} 