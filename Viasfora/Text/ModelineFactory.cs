using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IWpfTextViewCreationListener))]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [ContentType("text")]
  public class ModelineFactory : IWpfTextViewCreationListener {
    public void TextViewCreated(IWpfTextView textView) {
      if ( VsfSettings.ModelinesEnabled ) {
        ModeLineProvider provider = new ModeLineProvider(textView);
        for ( int i = 0; i < VsfSettings.ModelinesNumLines; i++ ) {
          provider.ParseModeline(i);
        }
      }
    }
  }
}
