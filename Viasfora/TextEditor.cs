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

    public static int S_OK = 0;
    private static void CheckError(int hr, String operation) {
      if ( hr != S_OK ) {
        VsfPackage.LogInfo("{0} returned 0x{1:x8}", operation, hr);
        throw new InvalidOperationException(String.Format("{0} returned 0x{1:x8}", operation, hr));
      }
    }

  }
}
