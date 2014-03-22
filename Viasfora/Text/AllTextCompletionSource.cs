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
    private ITextStructureNavigator navigator;

    public AllTextCompletionSource(
          ITextBuffer buffer,
          ITextSearchService searchService,
          ITextStructureNavigator structureNavigator) {
      this.theBuffer = buffer;
      this.textSearch = searchService;
      this.navigator = structureNavigator;
    }
    public void Dispose() {
    }

    public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
      ITrackingPoint triggerPoint = session.GetTriggerPoint(theBuffer);
      ITrackingSpan prefixSpan = GetPrefixSpan(triggerPoint);

      var matches = FindMatchingWords(prefixSpan.GetText(theBuffer.CurrentSnapshot));
      var completions = new List<Completion>();
      foreach ( var match in matches ) {
        completions.Add(new Completion(match));
      }
      var set = new CompletionSet(
        moniker: "Text",
        displayName: "Text",
        applicableTo: prefixSpan,
        completions: completions,
        completionBuilders: null
        );
      completionSets.Add(set);
    }

    private IEnumerable<String> FindMatchingWords(String prefix) {
      FindData fd = new FindData(prefix, theBuffer.CurrentSnapshot);
      HashSet<String> matches = new HashSet<string>();
      SnapshotSpan? span = textSearch.FindNext(0, false, fd);
      while ( span != null ) {
        String text = span.Value.GetText();
        if ( !matches.Contains(text) ) {
          yield return text;
          matches.Add(text);
        }
      }
    }

    private ITrackingSpan GetPrefixSpan(ITrackingPoint triggerPoint) {
      ITextSnapshot snapshot = theBuffer.CurrentSnapshot;
      int position = triggerPoint.GetPosition(snapshot);
      var extent = navigator.GetExtentOfWord(new SnapshotPoint(snapshot, position));
      return snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
    }
  }
}
