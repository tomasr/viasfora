using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(ICompletionSourceProvider))]
  [ContentType("plainText")]
  [Name("viasfora.text.completion")]
  public class AllTextCompletionSourceProvider : ICompletionSourceProvider {
    [Import]
    private ITextSearchService searchService = null;
    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
      return new AllTextCompletionSource(textBuffer, searchService);
    }
  }
}
