using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Winterdom.Viasfora.Text {
  public class PlainTextCompletionSource : ICompletionSource {
    private ITextBuffer theBuffer;
    private ITextSearchService textSearch;
    private ITextStructureNavigator navigator;
    private ImageSource glyphIcon;
    private List<Completion> currentCompletions;

    public PlainTextCompletionSource(
          ITextBuffer buffer,
          ITextSearchService searchService,
          ITextStructureNavigator structureNavigator) {
      this.theBuffer = buffer;
      this.textSearch = searchService;
      this.navigator = structureNavigator;
      glyphIcon = new BitmapImage(new Uri("pack://application:,,,/Winterdom.Viasfora;component/Resources/TextSource.png"));
    }
    public void Dispose() {
    }

    public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
      if ( !VsfSettings.TextCompletionEnabled ) {
        return;
      }
      // HACK: A complex editor such as htmlx can have multiple projection
      // buffers underneath the primary buffer of the view
      // that do *not* have the projection content type.
      // We'd only like to parse out the primary buffer
      if ( !TextEditor.IsNonProjectionOrElisionBuffer(this.theBuffer) ) {
        return;
      }

      var snapshot = theBuffer.CurrentSnapshot;
      ITrackingPoint triggerPoint = session.GetTriggerPoint(theBuffer);
      ITrackingSpan prefixSpan = GetPrefixSpan(triggerPoint);
      /*
      String prefix = prefixSpan.GetText(theBuffer.CurrentSnapshot);
      prefix = prefix.Trim();
      */

      bool refreshCompletions = false;
      if ( currentCompletions == null ) {
        currentCompletions = new List<Completion>();
        refreshCompletions = true;
      } else {
        refreshCompletions = IsSignificantChange(snapshot.Version);
      }

      if ( refreshCompletions ) {
        currentCompletions.Clear();
        var matches = FindMatchingWords()
                     .Distinct()
                     .OrderBy(x => x);
        foreach ( var match in matches ) {
          currentCompletions.Add(new Completion(match, match, match, glyphIcon, null));
        }
      }
      var set = new CompletionSet(
        moniker: "Text",
        displayName: "Text",
        applicableTo: prefixSpan,
        completions: currentCompletions,
        completionBuilders: null
        );
      completionSets.Add(set);
    }

    private bool IsSignificantChange(ITextVersion textVersion) {
      var changes = textVersion.Changes;
      if ( changes == null || changes.Count == 0 )
        return false;
      return changes.Any(change => change.Delta > 10);
    }

    private IEnumerable<String> FindMatchingWords() {
      var snapshot = theBuffer.CurrentSnapshot;
      SnapshotPoint pt = new SnapshotPoint(snapshot, 0);
      while ( pt.Position < snapshot.Length ) {
        var extent = navigator.GetExtentOfWord(pt);
        if ( extent != null ) {
          String text;
          if ( IsSignificant(extent, out text) ) {
            yield return extent.Span.GetText();
          }
          if ( extent.Span.End > pt )
            pt = extent.Span.End;
        }
        if ( pt.Position < snapshot.Length )
          pt += 1;
      }
    }

    private bool IsSignificant(TextExtent extent, out String word) {
      word = "";
      if ( !extent.IsSignificant ) return false;
      if ( extent.Span.IsEmpty ) return false;
      char ch = extent.Span.Start.GetChar();
      if ( Char.IsLetter(ch) || ch == '_' ) {
        word = extent.Span.GetText();
        return true;
      }
      return false;
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
