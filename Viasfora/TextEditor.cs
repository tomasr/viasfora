using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Editor;
using Winterdom.Viasfora.Compatibility;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using VsOle = Microsoft.VisualStudio.OLE.Interop;
using System.IO;

namespace Winterdom.Viasfora {
  public static class TextEditor {
    public static ITextCaret GetCurrentCaret() {
      ITextView view = GetCurrentView();
      if ( view == null ) {
        return null;
      }
      return view.Caret;
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

      IVsTextView textView;
      int hr = textManager.GetActiveView(1, null, out textView);
      CheckError(hr, "GetActiveView");

      var componentModel = new SComponentModel();
      var factory = componentModel.GetService<IVsEditorAdaptersFactoryService>();
      return factory.GetWpfTextView(textView);
    }

    public static String GetFileName(ITextBuffer buffer) {
      IVsTextBuffer adapter;
      if ( buffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out adapter) ) {
        IPersistFileFormat pff = adapter as IPersistFileFormat;
        if ( pff != null ) {
          String filename;
          uint formatIndex;
		      try {
            int hr = pff.GetCurFile(out filename, out formatIndex);
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
        VsfPackage.LogInfo("Could not find a primary buffer on view: {0}", view);
        return view.BufferGraph.TopBuffer;
      }
      return buffers[0];
    }

    public static bool IsPrimaryBufferType(Type type) {
      return type.FullName == "Microsoft.VisualStudio.Text.Implementation.TextBuffer";
    }

    //
    // Ugly hack: We write the buffer contents into a
    // temporary file, then open this in the standard
    // text editor as an "external" file and
    // then mark the buffer as read-only
    //
    public static void OpenBufferInPlainTextEditorAsReadOnly(ITextBuffer buffer) {
      String filepath = SaveBufferToTempPath(buffer);

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
      IVsTextLines textLines;
      if ( textView.GetBuffer(out textLines) == Constants.S_OK ) {
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
      IVsHierarchy hierarchy;
      uint itemid;
      uint documentCookie;
      IntPtr docData;

      int hr = docTable.FindAndLockDocument(
        dwRDTLockType: lockType,
        pszMkDocument: moniker,
        ppHier: out hierarchy,
        pitemid: out itemid,
        ppunkDocData: out docData,
        pdwCookie: out documentCookie
        );
      CheckError(hr, "FindAndLockDocument");
      docTable.ModifyDocumentFlags(documentCookie, lockType, 1);
    }

    private static string SaveBufferToTempPath(ITextBuffer buffer) {
      String tempDir = Path.GetTempPath();
      String file = Path.Combine(tempDir, Path.GetRandomFileName() + ".txt");
      File.WriteAllText(file, buffer.CurrentSnapshot.GetText());
      return file;
    }

    private static void CheckError(int hr, String operation) {
      if ( hr != Constants.S_OK ) {
        VsfPackage.LogInfo("{0} returned 0x{1:x8}", operation, hr);
        throw new InvalidOperationException(String.Format("{0} returned 0x{1:x8}", operation, hr));
      }
    }

  }
}
