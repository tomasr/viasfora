using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(ICompletionSourceProvider))]
  [ContentType("text"), ContentType("projection")]
  [Name("viasfora.text.completion.source")]
  [Order(After="default")]
  public class PlainTextCompletionSourceProvider : ICompletionSourceProvider {
    [Import]
    private ITextSearchService searchService = null;
    [Import]
    private ITextStructureNavigatorSelectorService navigatorSelector = null;
    [Import]
    private IContentTypeRegistryService ctRegistry = null;
    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
      var ct = ctRegistry.GetContentType("any");
      var navigator = navigatorSelector.CreateTextStructureNavigator(textBuffer, ct);
      return new PlainTextCompletionSource(textBuffer, searchService, navigator);
    }
  }
}
