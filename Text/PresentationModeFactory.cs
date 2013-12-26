using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Zoomable)]
  public class PresentationModeFactory : IWpfTextViewCreationListener {
    public void TextViewCreated(IWpfTextView textView) {
      textView.Properties.GetOrCreateSingletonProperty(
        () => new PresentationMode(textView)
      );
    }
  }
}
