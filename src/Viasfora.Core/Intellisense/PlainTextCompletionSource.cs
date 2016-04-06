using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace Winterdom.Viasfora.Intellisense {
  public sealed class PlainTextCompletionSource : ICompletionSource {
    private ITextBuffer theBuffer;
    private ITextStructureNavigator navigator;
    private IVsfSettings settings;
    private ImageSource glyphIcon;
    private IList<Completion> currentCompletions;
    private BufferStats bufferStatsOnCompletion;

    public PlainTextCompletionSource(
          ITextBuffer buffer,
          ITextStructureNavigator structureNavigator,
          IVsfSettings settings) {
      this.theBuffer = buffer;
      this.navigator = structureNavigator;
      this.settings = settings;
      glyphIcon = new BitmapImage(new Uri("pack://application:,,,/Winterdom.Viasfora;component/Resources/PlainTextCompletion.ico"));
    }
    public void Dispose() {
    }

    public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
      if ( !settings.TextCompletionEnabled ) {
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

      return newCompletions;
    }

    private IEnumerable<String> FindPlainTextWords() {
      // can't use a sorted-set here, because we want
      // uniqueness validated with case-sensitivity
      // but return the results case-insentitive sorted.
      HashSet<String> words = new HashSet<String>(StringComparer.Ordinal);
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
      return words.OrderBy(x=>x, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsSignificantText(TextExtent extent, out String word) {
      word = "";
      if ( !extent.IsSignificant ) return false;
      if ( extent.Span.IsEmpty ) return false;
      char ch = extent.Span.Start.GetChar();
      if ( Char.IsLetter(ch) || ch == '_' || ch == '$' ) {
        word = extent.Span.GetText();
        return true;
      }
      return false;
    }

    private ITrackingSpan GetApplicableToSpan(SnapshotPoint triggerPoint) {
      // triggerPoint is at the caret insertion point. That is, 
      // the character at triggerPoint is what is *after* the caret.
      // we want to obtain is the prefix before said position
      SnapshotSpan? word = null;
      ITextSnapshot snapshot = triggerPoint.Snapshot;
      if ( snapshot.Length == 0 ) {
        return snapshot.CreateTrackingSpan(new SnapshotSpan(triggerPoint, 0), SpanTrackingMode.EdgeInclusive);
      }
      SnapshotPoint end = triggerPoint;
      if ( end > 0 ) end -= 1;

      char ch = end.GetChar();
      if ( Char.IsWhiteSpace(ch) ) {
        word = new SnapshotSpan(end + 1, 0);
      } else if ( !IsIdentifierChar(ch) ) {
        word = new SnapshotSpan(end + 1, 0);
      } else {
        var extent = navigator.GetExtentOfWord(end);
        if ( extent != null ) {
          word = extent.Span;
        }
      }
      if ( !word.HasValue ) {
        word = new SnapshotSpan(end, end);
      }
      return snapshot.CreateTrackingSpan(word.Value, SpanTrackingMode.EdgeInclusive);
    }

    private bool IsIdentifierChar(char ch) {
      return Char.IsLetterOrDigit(ch);
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
