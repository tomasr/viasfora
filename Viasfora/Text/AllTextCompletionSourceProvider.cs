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
  [ContentType("text")]
  [Name("viasfora.text.completion.source")]
  public class AllTextCompletionSourceProvider : ICompletionSourceProvider {
    [Import]
    private ITextSearchService searchService = null;
    [Import]
    private ITextStructureNavigatorSelectorService navigatorSelector = null;
    [Import]
    private IContentTypeRegistryService ctRegistry = null;
    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
      var ct = ctRegistry.GetContentType("any");
      var navigator = navigatorSelector.CreateTextStructureNavigator(textBuffer, ct);
      return new AllTextCompletionSource(textBuffer, searchService, navigator);
    }
  }
}
