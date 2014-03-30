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
    private ITextStructureNavigator navigator;
    private ImageSource glyphIcon;
    private List<Completion> currentCompletions;
    private int lastCompletionVersionNumber;

    public int CurrentVersion {
      get { return theBuffer.CurrentSnapshot.Version.VersionNumber; }
    }

    public PlainTextCompletionSource(
          ITextBuffer buffer,
          ITextStructureNavigator structureNavigator) {
      this.theBuffer = buffer;
      this.navigator = structureNavigator;
      glyphIcon = new BitmapImage(new Uri("pack://application:,,,/Winterdom.Viasfora;component/Resources/TextSource.png"));
    }
    public void Dispose() {
    }

    public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
      if ( !VsfSettings.TextCompletionEnabled ) {
        return;
      }
      if ( session.TextView.TextBuffer != this.theBuffer ) {
        return;
      }
      if ( !PlainTextCompletionContext.IsSet(session) ) {
        return;
      }
      var snapshot = theBuffer.CurrentSnapshot;
      var triggerPoint = session.GetTriggerPoint(snapshot);
      if ( !triggerPoint.HasValue ) {
        return;
      }

      var applicableToSpan = GetApplicableToSpan(triggerPoint.Value);

      bool refreshCompletions = false;
      if ( currentCompletions == null ) {
        refreshCompletions = true;
      } else {
        refreshCompletions = IsSignificantChange(snapshot.Version);
      }

      if ( refreshCompletions ) {
        BuildCompletionsList();
      }
      var set = new CompletionSet(
        moniker: "plainText",
        displayName: "Text",
        applicableTo: applicableToSpan,
        completions: currentCompletions,
        completionBuilders: null
        );
      completionSets.Add(set);
    }

    private void BuildCompletionsList() {
      var words = FindPlainTextWords().Distinct();
      var newCompletions = 
        (from w in words
         select new Completion(w, w, w, glyphIcon, null))
         .ToList();

      newCompletions.Sort(CompletionComparer.Instance);
      this.currentCompletions = newCompletions;
      lastCompletionVersionNumber = CurrentVersion;
    }

    private bool IsSignificantChange(ITextVersion textVersion) {
      if ( textVersion.VersionNumber == this.lastCompletionVersionNumber )
        return false;
      var changes = textVersion.Changes;
      if ( changes != null && changes.Count > 0 ) {
        // to avoid having to reparse the document on every key stroke
        // we only do it if we consider the document has changed "enough":
        // - if the change affects the number of lines in the buffer
        if ( changes.Any(change => change.LineCountDelta > 0) )
          return true;
        // - if the change affects more than 10 characters
        if ( changes.Any(change => change.NewText.Length > 10) )
          return true;
        // TODO: Cut/Copy/Paste changes could significantly impact this
      }
      // - if we have ignored more than 10 previous changes
      return (textVersion.VersionNumber - this.lastCompletionVersionNumber) >= 10;
    }

    private IEnumerable<String> FindPlainTextWords() {
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

    private ITrackingSpan GetApplicableToSpan(SnapshotPoint triggerPoint) {
      ITextSnapshot snapshot = triggerPoint.Snapshot;
      SnapshotPoint end = triggerPoint;
      if ( end > 0 ) end -= 1;
      var word = navigator.GetExtentOfWord(end);
      return snapshot.CreateTrackingSpan(word.Span, SpanTrackingMode.EdgeInclusive);
    }

    private bool IsIdentifierCharacter(char ch) {
      return Char.IsLetterOrDigit(ch)
        || ch == '_' || ch == '-';
    }

    private class CompletionComparer : IComparer<Completion> {
      private StringComparer comparer = StringComparer.OrdinalIgnoreCase;
      public static CompletionComparer Instance = new CompletionComparer();
      public int Compare(Completion x, Completion y) {
        return comparer.Compare(x.DisplayText, y.DisplayText);
      }
    }
  }
}
