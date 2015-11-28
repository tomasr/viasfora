using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Tags;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {

  class KeywordTagger : ITagger<KeywordTag>, IDisposable {
    private ITextBuffer theBuffer;
    private KeywordTag keywordClassification;
    private KeywordTag linqClassification;
    private KeywordTag visClassification;
    private KeywordTag stringEscapeClassification;
    private KeywordTag formatSpecClassification;
    private ITagAggregator<IClassificationTag> aggregator;
    private ILanguageFactory langFactory;
    private IVsfSettings settings;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal KeywordTagger(ITextBuffer buffer, KeywordTaggerProvider provider) {
      theBuffer = buffer;
      this.aggregator = provider.Aggregator.CreateTagAggregator<IClassificationTag>(buffer);
      this.langFactory = provider.LanguageFactory;

      var registry = provider.ClassificationRegistry;
      keywordClassification =
         new KeywordTag(registry.GetClassificationType(Constants.KEYWORD_CLASSIF_NAME));
      linqClassification =
         new KeywordTag(registry.GetClassificationType(Constants.LINQ_CLASSIF_NAME));
      visClassification =
         new KeywordTag(registry.GetClassificationType(Constants.VISIBILITY_CLASSIF_NAME));
      stringEscapeClassification =
         new KeywordTag(registry.GetClassificationType(Constants.STRING_ESCAPE_CLASSIF_NAME));
      formatSpecClassification =
         new KeywordTag(registry.GetClassificationType(Constants.FORMAT_SPECIFIER_NAME));

      this.settings = provider.Settings;
      this.settings.SettingsChanged += this.OnSettingsChanged;
    }

    public IEnumerable<ITagSpan<KeywordTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        yield break;
      }
      ILanguage lang = GetKeywordsByContentType(theBuffer.ContentType);
      if ( !lang.Enabled ) {
        yield break;
      }

      bool eshe = settings.EscapeSeqHighlightEnabled;
      bool kce = settings.KeywordClassifierEnabled;
      if ( !(kce || eshe) ) {
        yield break;
      }

      ITextSnapshot snapshot = spans[0].Snapshot;
      foreach ( var tagSpan in aggregator.GetTags(spans) ) {
        if ( tagSpan.Tag.ClassificationType is RainbowTag )
          continue;
        String name = tagSpan.Tag.ClassificationType.Classification.ToLower();
        if ( eshe && name.Contains("string") ) {
          var span = tagSpan.GetSpan(snapshot);
          foreach ( var escapeTag in ProcessEscapeSequences(lang, span) ) {
            yield return escapeTag;
          }
        }

        if ( kce && name.Contains("keyword") ) {
          var span = tagSpan.GetSpan(snapshot);
          // Is this one of the keywords we care about?
          var result = IsInterestingKeyword(lang, span);
          if ( result != null ) {
            yield return result;
          }
        }
      }
    }

    public void Dispose() {
      if ( settings != null ) {
        settings.SettingsChanged -= OnSettingsChanged;
        settings = null;
      }
      if ( aggregator != null ) {
        aggregator.Dispose();
        aggregator = null;
      }
      theBuffer = null;
    }
    void OnSettingsChanged(object sender, EventArgs e) {
      if ( this.theBuffer == null )
        return;
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(theBuffer.CurrentSnapshot, 0,
            theBuffer.CurrentSnapshot.Length)));
      }
    }

    private ITagSpan<KeywordTag> IsInterestingKeyword(ILanguage lang, SnapshotSpan cs) {
      if ( cs.IsEmpty ) return null;
      String text = cs.GetText();
      if ( lang.IsControlFlowKeyword(text) ) {
        return new TagSpan<KeywordTag>(cs, keywordClassification);
      } else if ( lang.IsVisibilityKeyword(text) ) {
        return new TagSpan<KeywordTag>(cs, visClassification);
      } else if ( lang.IsLinqKeyword(text) ) {
        return new TagSpan<KeywordTag>(cs, linqClassification);
      }
      return null;
    }

    private IEnumerable<ITagSpan<KeywordTag>> ProcessEscapeSequences(
          ILanguage lang, SnapshotSpan cs) {
      if ( cs.IsEmpty ) yield break;
      String text = cs.GetText();

      var parser = lang.NewStringParser(text);
      if ( parser == null )
        yield break;

      StringPart? part;
      while ( (part = parser.Next()) != null ) {
        var span = part.Value.Span;
        var sspan = new SnapshotSpan(cs.Snapshot, cs.Start.Position + span.Start, span.Length);
        switch ( part.Value.Type ) {
          case StringPartType.EscapeSequence:
            yield return new TagSpan<KeywordTag>(sspan, stringEscapeClassification);
            break;
          case StringPartType.FormatSpecifier:
            yield return new TagSpan<KeywordTag>(sspan, formatSpecClassification);
            break;
        }
      }
    }

    private ILanguage GetKeywordsByContentType(IContentType contentType) {
      return this.langFactory.TryCreateLanguage(contentType);
    }
  }
}
