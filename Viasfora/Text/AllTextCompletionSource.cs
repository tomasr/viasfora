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
        completions.Add(new Completion(match, match, match, null, null));
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
      StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
      FindData fd = new FindData(
        PrefixToRegEx(prefix),
        theBuffer.CurrentSnapshot,
        FindOptions.UseRegularExpressions,
        navigator
        );
      HashSet<String> matches = new HashSet<string>();
      SnapshotSpan? span = textSearch.FindNext(0, false, fd);
      while ( span != null ) {
        String text = span.Value.GetText();
        if ( !matches.Contains(text) && comparer.Compare(text, prefix) != 0 ) {
          //yield return text;
          matches.Add(text);
        }
        span = textSearch.FindNext(span.Value.End, false, fd);
      }
      return matches;
    }

    private string PrefixToRegEx(string prefix) {
      return String.Format(
        @"\b{0}(_\w+|[\w-[0-9_]]\w*)\b",
        System.Text.RegularExpressions.Regex.Escape(prefix));
    }

    private ITrackingSpan GetPrefixSpan(ITrackingPoint triggerPoint) {
      ITextSnapshot snapshot = theBuffer.CurrentSnapshot;
      int position = triggerPoint.GetPosition(snapshot);
      if ( position > 0 ) position--; 
      var extent = navigator.GetExtentOfWord(new SnapshotPoint(snapshot, position));
      return snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
    }
  }
}
