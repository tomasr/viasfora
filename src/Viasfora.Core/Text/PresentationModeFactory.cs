using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("any")]
  [TextViewRole(PredefinedTextViewRoles.Interactive)]
  public class PresentationModeFactory : IWpfTextViewCreationListener {
    [Import]
    internal IVsfSettings Settings { get; set; }
    [Import]
    internal IPresentationModeState State { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      textView.Properties.GetOrCreateSingletonProperty(
        () => new PresentationMode(textView, State, Settings)
      );
    }
  }
}
