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
    private IList<Completion> currentCompletions;
    private BufferStats bufferStatsOnCompletion;

    public PlainTextCompletionSource(
          ITextBuffer buffer,
          ITextStructureNavigator structureNavigator) {
      this.theBuffer = buffer;
      this.navigator = structureNavigator;
      glyphIcon = new BitmapImage(new Uri("pack://application:,,,/Winterdom.Viasfora;component/Resources/PlainTextCompletion.ico"));
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

      var then = this.bufferStatsOnCompletion;
      var now = new BufferStats(snapshot);
      if ( currentCompletions == null || now.SignificantThan(then) ) {
        this.currentCompletions = BuildCompletionsList();
        this.bufferStatsOnCompletion = now;
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

    private IList<Completion> BuildCompletionsList() {
      var words = FindPlainTextWords();
      var newCompletions =
        (from w in words
         select new Completion(w, w, w, glyphIcon, null))
         .ToList();

      //newCompletions.Sort(CompletionComparer.Instance);
      return newCompletions;
    }

    private IEnumerable<String> FindPlainTextWords() {
      SortedSet<String> words = new SortedSet<String>(StringComparer.OrdinalIgnoreCase);
      var snapshot = theBuffer.CurrentSnapshot;
      SnapshotPoint pt = new SnapshotPoint(snapshot, 0);
      while ( pt.Position < snapshot.Length ) {
        var extent = navigator.GetExtentOfWord(pt);
        if ( extent != null ) {
          String text;
          if ( IsSignificantText(extent, out text) ) {
            words.Add(text);
          }
          if ( extent.Span.End > pt )
            pt = extent.Span.End;
        }
        if ( pt.Position < snapshot.Length )
          pt += 1;
      }
      return words;
    }

    private bool IsSignificantText(TextExtent extent, out String word) {
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

    private struct BufferStats {
      private int version;
      private int lineCount;
      public int Version {
        get { return version; }
      }
      public int LineCount {
        get { return lineCount; }
      }

      public BufferStats(ITextSnapshot snapshot) {
        this.version = snapshot.Version.VersionNumber;
        this.lineCount = snapshot.LineCount;
      }

      public bool SignificantThan(BufferStats then) {
        // if a change has added lines, rebuild it
        if ( this.LineCount > then.LineCount ) {
          return true;
        }
        // has there been more than a 100 version changes?
        if ( (this.Version - then.Version) > 100 ) {
          return true;
        }
        return false;
      }
    }
  }
}
