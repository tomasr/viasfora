using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IWpfTextViewCreationListener))]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [ContentType("text")]
  public class ModelineFactory : IWpfTextViewCreationListener {
    [Import]
    public ILanguageFactory LanguageFactory { get; set; }
    [Import]
    public IVsfSettings Settings { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      if ( Settings.ModelinesEnabled ) {
        ModeLineProvider provider = new ModeLineProvider(textView, this);
        for ( int i = 0; i < Settings.ModelinesNumLines; i++ ) {
          provider.ParseModeline(i);
        }
      }
    }
  }
}
