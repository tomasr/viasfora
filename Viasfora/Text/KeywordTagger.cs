using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Text {

  class KeywordTagger : ITagger<KeywordTag>, IDisposable {
    private ITextBuffer theBuffer;
    private KeywordTag keywordClassification;
    private KeywordTag linqClassification;
    private KeywordTag visClassification;
    private KeywordTag stringEscapeClassification;
    private ITagAggregator<IClassificationTag> aggregator;

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal KeywordTagger(
          ITextBuffer buffer,
          IClassificationTypeRegistryService registry,
          ITagAggregator<IClassificationTag> aggregator) {
      theBuffer = buffer;
      keywordClassification =
         new KeywordTag(registry.GetClassificationType(Constants.KEYWORD_CLASSIF_NAME));
      linqClassification =
         new KeywordTag(registry.GetClassificationType(Constants.LINQ_CLASSIF_NAME));
      visClassification =
         new KeywordTag(registry.GetClassificationType(Constants.VISIBILITY_CLASSIF_NAME));
      stringEscapeClassification =
         new KeywordTag(registry.GetClassificationType(Constants.STRING_ESCAPE_CLASSIF_NAME));

      VsfSettings.SettingsUpdated += this.OnSettingsUpdated;
      this.aggregator = aggregator;
    }

    public IEnumerable<ITagSpan<KeywordTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        yield break;
      }
      LanguageInfo lang = GetKeywordsByContentType(theBuffer.ContentType);
      bool eshe = VsfSettings.EscapeSeqHighlightEnabled;
      bool kce = VsfSettings.KeywordClassifierEnabled;
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
      if ( theBuffer != null ) {
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        theBuffer = null;
      }
      if ( aggregator != null ) {
        aggregator.Dispose();
        aggregator = null;
      }
    }
    void OnSettingsUpdated(object sender, EventArgs e) {
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(theBuffer.CurrentSnapshot, 0,
            theBuffer.CurrentSnapshot.Length)));
      }
    }

    private ITagSpan<KeywordTag> IsInterestingKeyword(LanguageInfo lang, SnapshotSpan cs) {
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
          LanguageInfo lang, SnapshotSpan cs) {
      if ( cs.IsEmpty ) yield break;
      String text = cs.GetText();

      var parser = lang.NewEscapeSequenceParser(text);
      if ( parser == null )
        yield break;

      Span? span;
      while ( (span = parser.Next()) != null ) {
        var sspan = new SnapshotSpan(cs.Snapshot, cs.Start.Position + span.Value.Start, span.Value.Length);
        yield return new TagSpan<KeywordTag>(sspan, stringEscapeClassification);
      }
    }

    private bool IsHexDigit(char c) {
      if ( Char.IsDigit(c) ) return true;
      return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }

    private LanguageInfo GetKeywordsByContentType(IContentType contentType) {
      return VsfPackage.LookupLanguage(contentType);
    }
  }
}
