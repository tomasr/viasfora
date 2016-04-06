using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Intellisense {
  [Export(typeof(ICompletionSourceProvider))]
  [ContentType(ContentTypes.Text)]
  [ContentType(ContentTypes.Projection)]
  [Name("viasfora.text.completion.source")]
  [Order(After="default")]
  public class PlainTextCompletionSourceProvider : ICompletionSourceProvider {
    [Import]
    private ITextStructureNavigatorSelectorService navigatorSelector = null;
    [Import]
    private IContentTypeRegistryService ctRegistry = null;
    [Import]
    private IVsfSettings settings = null;
    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
      var ct = ctRegistry.GetContentType("any");
      var navigator = navigatorSelector.CreateTextStructureNavigator(textBuffer, ct);
      return new PlainTextCompletionSource(textBuffer, navigator, settings);
    }
  }
}
