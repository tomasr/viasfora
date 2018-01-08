using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace Winterdom.Viasfora.XLangSupport {
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType(ContentTypes.Code)]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class XLangTextViewCreationListener : IWpfTextViewCreationListener {
    public static readonly Guid FakeTag = new Guid("647e2e59-802b-44f4-af39-191b3c9e939d");

    [Import]
    public IContentTypeRegistryService Registry { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      // XLANG/s windows in the orchestration editor
      // are created once per VS session.
      // They contain a TextBuffer with "code" content type
      // that is empty on creation
      ITextBuffer buffer = textView.TextBuffer;
      if ( buffer.ContentType.TypeName != ContentTypes.Code )
        return;

      if ( buffer.CurrentSnapshot.Length == 0 ) {
        ChangeContentType(buffer);
      }
    }

    private void ChangeContentType(ITextBuffer buffer) {
      IContentType fakeXLang = 
        Registry.GetContentType(ContentTypes.XLang);
      buffer.ChangeContentType(fakeXLang, FakeTag);
    }
  }
}
