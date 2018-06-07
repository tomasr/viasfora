using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Tags;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {

  class KeywordTagger : ITagger<KeywordTag>, IDisposable {
    private ITextBuffer theBuffer;
    private KeywordTag keywordClassification;
    private KeywordTag linqClassification;
    private KeywordTag visClassification;
    private KeywordTag stringEscapeClassification;
    private KeywordTag stringEscapeErrorClassification;
    private KeywordTag formatSpecClassification;
    private ITagAggregator<IClassificationTag> aggregator;
    private ILanguageFactory langFactory;
    private IVsfSettings settings;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal KeywordTagger(ITextBuffer buffer, KeywordTaggerProvider provider) {
      this.theBuffer = buffer;
      this.aggregator = provider.Aggregator.CreateTagAggregator<IClassificationTag>(buffer);
      this.langFactory = provider.LanguageFactory;

      this.keywordClassification = provider.GetTag(Constants.FLOW_CONTROL_CLASSIF_NAME);
      this.linqClassification = provider.GetTag(Constants.LINQ_CLASSIF_NAME);
      this.visClassification = provider.GetTag(Constants.VISIBILITY_CLASSIF_NAME);
      this.stringEscapeClassification = provider.GetTag(Constants.STRING_ESCAPE_CLASSIF_NAME);
      this.stringEscapeErrorClassification = provider.GetTag(Constants.STRING_ESCAPE_ERROR_NAME);
      this.formatSpecClassification = provider.GetTag(Constants.FORMAT_SPECIFIER_NAME);

      this.settings = provider.Settings;
      this.settings.SettingsChanged += this.OnSettingsChanged;
    }

    public IEnumerable<ITagSpan<KeywordTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        yield break;
      }
      ILanguage lang = GetLanguageByContentType(this.theBuffer.ContentType);
      if ( !lang.Settings.Enabled ) {
        yield break;
      }

      bool eshe = this.settings.EscapeSequencesEnabled;
      bool kce = this.settings.KeywordClassifierEnabled;
      if ( !(kce || eshe) ) {
        yield break;
      }

      ITextSnapshot snapshot = spans[0].Snapshot;

      // Get all spans that contain interesting tags
      // translated into our snapshot
      var interestingSpans = from tagSpan in this.aggregator.GetTags(spans)
                             let classificationType = tagSpan.Tag.ClassificationType
                             where IsInterestingTag(lang, classificationType)
                             select tagSpan.ToTagSpan(snapshot);

      // GetTags() coalesce adjacent spans with the same tag
      // so that we can process them as a single span
      foreach ( var tagSpan in GetTags(interestingSpans, snapshot) ) {
        var classificationType = tagSpan.Tag.ClassificationType;
        String name = classificationType.Classification.ToLower();
        if ( eshe && name.Contains("string") ) {
          foreach ( var escapeTag in ProcessEscapeSequences(lang, name, tagSpan.Span) ) {
            yield return escapeTag;
          }
        }

        if ( kce && lang.IsKeywordClassification(classificationType.Classification) ) {
          // Is this one of the keywords we care about?
          var result = IsInterestingKeyword(lang, tagSpan.Span);
          if ( result != null ) {
            yield return result;
          }
        }
      }
    }

    private bool IsInterestingTag(ILanguage lang, IClassificationType classification) {
      if ( classification is RainbowTag )
        return false;
      var name = classification.Classification;
      if ( this.settings.EscapeSequencesEnabled && name.IndexOf("string", StringComparison.InvariantCultureIgnoreCase) >= 0 )
        return true;
      if ( this.settings.KeywordClassifierEnabled && lang.IsKeywordClassification(classification.Classification) )
        return true;
      return false;
    }

    private IEnumerable<ITagSpan<IClassificationTag>> GetTags(IEnumerable<ITagSpan<IClassificationTag>> sourceSpans, ITextSnapshot snapshot) {
      var e = sourceSpans.GetEnumerator();
      try {
        IClassificationTag currentTag = null;
        SnapshotSpan currentSpan = new SnapshotSpan();
        while ( e.MoveNext() ) {
          var c1 = e.Current;
          currentSpan = c1.Span;
          currentTag = c1.Tag;
          while ( e.MoveNext() ) {
            var c2 = e.Current;
            if ( IsSameTag(currentTag, c2) && AreAdjacent(currentSpan, c2) ) {
              currentSpan = new SnapshotSpan(currentSpan.Start, c2.Span.End - currentSpan.Start);
            } else {
              yield return c1;
              yield return c2;
            }
          }
          yield return new TagSpan<IClassificationTag>(currentSpan, currentTag);
        }
      } finally {
        e.Dispose();
      }
    }

    private bool AreAdjacent(SnapshotSpan c1, ITagSpan<IClassificationTag> c2) {
      return c1.End == c2.Span.Start;
    }

    private bool IsSameTag(IClassificationTag c1, ITagSpan<IClassificationTag> c2) {
      return c1.ClassificationType.Classification == c2.Tag.ClassificationType.Classification;
    }

    public void Dispose() {
      if ( this.settings != null ) {
        this.settings.SettingsChanged -= OnSettingsChanged;
        this.settings = null;
      }
      if ( this.aggregator != null ) {
        this.aggregator.Dispose();
        this.aggregator = null;
      }
      this.theBuffer = null;
    }
    void OnSettingsChanged(object sender, EventArgs e) {
      if ( this.theBuffer == null )
        return;
      TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(this.theBuffer.CurrentSnapshot.GetSpan()));
    }

    private ITagSpan<KeywordTag> IsInterestingKeyword(ILanguage lang, SnapshotSpan cs) {
      if ( cs.IsEmpty ) return null;
      String text = cs.GetText();
      if ( this.settings.FlowControlKeywordsEnabled && lang.IsControlFlowKeyword(text) ) {
        return new TagSpan<KeywordTag>(cs, this.keywordClassification);
      } else if ( this.settings.VisibilityKeywordsEnabled && lang.IsVisibilityKeyword(text) ) {
        return new TagSpan<KeywordTag>(cs, this.visClassification);
      } else if ( this.settings.QueryKeywordsEnabled && lang.IsLinqKeyword(text) ) {
        return new TagSpan<KeywordTag>(cs, this.linqClassification);
      }
      return null;
    }

    private IEnumerable<ITagSpan<KeywordTag>> ProcessEscapeSequences(
          ILanguage lang, String classificationName, SnapshotSpan cs) {
      if ( cs.IsEmpty ) yield break;
      String text = cs.GetText();

      var parser = lang.NewStringScanner(classificationName, text);
      if ( parser == null )
        yield break;

      StringPart? part;
      while ( (part = parser.Next()) != null ) {
        var span = part.Value.Span;
        var sspan = new SnapshotSpan(cs.Snapshot, cs.Start.Position + span.Start, span.Length);
        switch ( part.Value.Type ) {
          case StringPartType.EscapeSequence:
            yield return new TagSpan<KeywordTag>(sspan, this.stringEscapeClassification);
            break;
          case StringPartType.FormatSpecifier:
            yield return new TagSpan<KeywordTag>(sspan, this.formatSpecClassification);
            break;
          case StringPartType.EscapeSequenceError:
            yield return new TagSpan<KeywordTag>(sspan, this.stringEscapeErrorClassification);
            break;
        }
      }
    }

    private ILanguage GetLanguageByContentType(IContentType contentType) {
      return this.langFactory.TryCreateLanguage(contentType);
    }
  }
}
