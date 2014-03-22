using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace Winterdom.Viasfora.Text {
  public class AllTextCompletionSource : ICompletionSource {
    private ITextBuffer theBuffer;
    private ITextSearchService textSearch;

    public AllTextCompletionSource(ITextBuffer buffer, ITextSearchService searchService) {
      this.theBuffer = buffer;
      this.textSearch = searchService;
    }
    public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
    }

    public void Dispose() {
    }
  }
}
