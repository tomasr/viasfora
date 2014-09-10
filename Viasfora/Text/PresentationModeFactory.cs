using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("any")]
  [TextViewRole(PredefinedTextViewRoles.Interactive)]
  public class PresentationModeFactory : IWpfTextViewCreationListener {
    [Import]
    internal IVsfSettings Settings { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      textView.Properties.GetOrCreateSingletonProperty(
        () => new PresentationMode(textView, Settings)
      );
    }
  }
}
