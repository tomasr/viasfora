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

      var snapshot = theBuffer.CurrentSnapshot;
      var applicableToSpan = GetBufferSpan(snapshot);
      ITrackingPoint triggerPoint = session.GetTriggerPoint(theBuffer);
      ITrackingSpan prefixSpan = GetPrefixSpan(triggerPoint);

      bool refreshCompletions = false;
      if ( currentCompletions == null ) {
        currentCompletions = new List<Completion>();
        refreshCompletions = true;
      } else {
        refreshCompletions = IsSignificantChange(snapshot.Version);
      }

      if ( refreshCompletions ) {
        lastCompletionVersionNumber = CurrentVersion;
        currentCompletions.Clear();
        var matches = FindMatchingWords()
                     .Distinct()
                     .OrderBy(x => x);
        foreach ( var match in matches ) {
          currentCompletions.Add(new Completion(match, match, match, glyphIcon, null));
        }
      }
      var set = new CompletionSet(
        moniker: "plainText",
        displayName: "Text",
        applicableTo: prefixSpan,
        completions: currentCompletions,
        completionBuilders: null
        );
      completionSets.Add(set);
    }

    private ITrackingSpan GetBufferSpan(ITextSnapshot snapshot) {
      return snapshot.CreateTrackingSpan(0, snapshot.Length, SpanTrackingMode.EdgeInclusive);
    }

    private bool IsSignificantChange(ITextVersion textVersion) {
      var changes = textVersion.Changes;
      if ( changes == null || changes.Count == 0 )
        return false;
      if ( textVersion.VersionNumber == this.lastCompletionVersionNumber )
        return false;
      // to avoid having to reparse the document on every key stroke
      // we only do it if we consider the document has changed "enough":
      // - if the change affects the number of lines in the buffer
      if ( changes.Any(change => change.LineCountDelta > 0) )
        return true;
      // - if the change affects more than 10 characters
      if ( changes.Any(change => change.NewText.Length > 10) )
        return true;
      // - if we have ignored more than 20 previous changes
      return (textVersion.VersionNumber - this.lastCompletionVersionNumber) >= 20;
      // TODO: Cut/Copy/Paste changes could significantly impact this
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

    private ITrackingSpan GetPrefixSpan(ITrackingPoint triggerPoint) {
      ITextSnapshot snapshot = theBuffer.CurrentSnapshot;
      int position = triggerPoint.GetPosition(snapshot);
      if ( position > 0 ) position--;
      var extent = navigator.GetExtentOfWord(new SnapshotPoint(snapshot, position));
      return snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
    }
  }
}
