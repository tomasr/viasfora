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

    public static bool OpenBufferInPlainTextEditorAsReadOnly(ITextBuffer buffer) {
      String filepath = SaveBufferToTempPath(buffer);
      var uiShell = (IVsUIShellOpenDocument)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShellOpenDocument));
      var oleSvcProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
        ServiceProvider.GlobalProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider));

      IVsUIHierarchy hierarchy;
      uint itemid;
      int hr = CreateHierarchy(filepath, out hierarchy, out itemid);
      if ( hr != 0 ) return false;

      Guid editorType = VSConstants.VsEditorFactoryGuid.TextEditor_guid;
      Guid logicalView = VSConstants.LOGVIEWID.TextView_guid;
      String physicalView = "Code";

      IVsWindowFrame windowFrame = null;
      hr = uiShell.OpenSpecificEditor(
        grfOpenSpecific: (uint)0,
        pszMkDocument: filepath,
        rguidEditorType: ref editorType,
        pszPhysicalView: physicalView,
        rguidLogicalView: ref logicalView,
        pszOwnerCaption: Path.GetFileName(filepath),
        pHier: hierarchy,
        itemid: itemid,
        punkDocDataExisting: IntPtr.Zero,
        pSPHierContext: oleSvcProvider,
        ppWindowFrame: out windowFrame
      );
      CheckError(hr, "OpenSpecificEditor");
      MarkDocumentAsTemporaryAndShow(filepath, windowFrame);
      return true;
    }

    private static void MarkDocumentAsTemporaryAndShow(string moniker, IVsWindowFrame windowFrame) {
      IVsRunningDocumentTable docTable = (IVsRunningDocumentTable)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsRunningDocumentTable));

      uint lockType = (uint)_VSRDTFLAGS.RDT_CantSave
                    | (uint)_VSRDTFLAGS.RDT_DontAddToMRU
                    | (uint)_VSRDTFLAGS.RDT_DontAutoOpen
                    | (uint)_VSRDTFLAGS.RDT_NonCreatable
                    | (uint)_VSRDTFLAGS.RDT_VirtualDocument
                    | (uint)_VSRDTFLAGS.RDT_PlaceHolderDoc
                    | (uint)_VSRDTFLAGS.RDT_ReadLock
                    | (uint)_VSRDTFLAGS.RDT_EditLock;
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
      if ( windowFrame != null ) {
        windowFrame.Show();
      }
    }

    // based on: https://github.com/jaredpar/VsSamples/blob/master/Src/ProjectionBufferDemo/Implementation/EditorFactory.cs
    private static int CreateHierarchy(string moniker, out IVsUIHierarchy hierarchy, out uint itemId) {
      IVsExternalFilesManager filesMgr = (IVsExternalFilesManager)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsExternalFilesManager));

      int defaultPosition;
      IVsWindowFrame dummyWindowFrame;
      uint flags = (uint)_VSRDTFLAGS.RDT_NonCreatable | (uint)_VSRDTFLAGS.RDT_PlaceHolderDoc;
      var hr = filesMgr.AddDocument(
          dwCDW: flags,
          pszMkDocument: moniker,
          punkDocView: IntPtr.Zero,
          punkDocData: IntPtr.Zero,
          rguidEditorType: Guid.Empty,
          pszPhysicalView: null,
          rguidCmdUI: Guid.Empty,
          pszOwnerCaption: moniker,
          pszEditorCaption: null,
          pfDefaultPosition: out defaultPosition,
          ppWindowFrame: out dummyWindowFrame);
      ErrorHandler.ThrowOnFailure(hr);

      // Get the hierarchy for the document we added to the miscellaneous files project
      IVsProject vsProject;
      hr = filesMgr.GetExternalFilesProject(out vsProject);
      ErrorHandler.ThrowOnFailure(hr);

      int found;
      VSDOCUMENTPRIORITY[] priority = new VSDOCUMENTPRIORITY[1];
      hr = vsProject.IsDocumentInProject(moniker, out found, priority, out itemId);
      ErrorHandler.ThrowOnFailure(hr);
      if ( 0 == found || VSConstants.VSITEMID_NIL == itemId ) {
        throw new InvalidOperationException("Could not find in project");
      }

      hierarchy = (IVsUIHierarchy)vsProject;
      return 0;
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
