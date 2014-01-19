using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;

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

      var componentModel = GetComponentModel();
      if ( componentModel != null ) {
        var factory = componentModel.GetService<IVsEditorAdaptersFactoryService>();
        return factory.GetWpfTextView(textView);
      }
      return null;
    }

    public static IComponentModel GetComponentModel() {
      return (IComponentModel)
        ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
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
