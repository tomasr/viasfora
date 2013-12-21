using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Text {

  class KeywordTagger : ITagger<KeywordTag>, IDisposable {
    private ITextBuffer theBuffer;
    private KeywordTag keywordClassification;
    private KeywordTag linqClassification;
    private KeywordTag visClassification;
    private KeywordTag stringEscapeClassification;
    private IClassificationType[] rainbowTypes;
    private ITagAggregator<IClassificationTag> aggregator;
    private StringComparer comparer;

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
      rainbowTypes = RainbowClassifier.GetRainbows(registry, Constants.MAX_RAINBOW_DEPTH);

      VsfSettings.SettingsUpdated += this.OnSettingsUpdated;
      this.aggregator = aggregator;
      this.comparer = StringComparer.CurrentCultureIgnoreCase;
    }

    public IEnumerable<ITagSpan<KeywordTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        yield break;
      }
      LanguageInfo lang = GetKeywordsByContentType(theBuffer.ContentType);
      bool eshe = VsfSettings.EscapeSeqHighlightEnabled && lang.SupportsEscapeSeqs;
      bool kce = VsfSettings.KeywordClassifierEnabled;
      if ( !(kce || eshe) ) {
        yield break;
      }

      ITextSnapshot snapshot = spans[0].Snapshot;
      foreach ( var tagSpan in aggregator.GetTags(spans) ) {
        if ( this.rainbowTypes.Contains(tagSpan.Tag.ClassificationType) )
          continue;
        String name = tagSpan.Tag.ClassificationType.Classification.ToLower();
        if ( eshe && name.Contains("string") ) {
          var span = tagSpan.GetSpan(snapshot);
          foreach ( var escapeTag in ProcessEscapeSequences(span) ) {
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
      if ( lang.ControlFlow.Contains(text, comparer) ) {
        return new TagSpan<KeywordTag>(cs, keywordClassification);
      } else if ( lang.Visibility.Contains(text, comparer) ) {
        return new TagSpan<KeywordTag>(cs, visClassification);
      } else if ( lang.Linq.Contains(text, comparer) ) {
        return new TagSpan<KeywordTag>(cs, linqClassification);
      }
      return null;
    }

    private IEnumerable<ITagSpan<KeywordTag>> ProcessEscapeSequences(SnapshotSpan cs) {
      if ( cs.IsEmpty ) yield break;

      String text = cs.GetText();
      // don't process verbatim strings
      if ( text.StartsWith("@") || text.StartsWith("<") ) yield break;
      int start = 1;
      while ( start < text.Length - 2 ) {
        if ( text[start] == '\\' ) {
          int len = 1;
          int maxlen = Int32.MaxValue;
          char f = text[start + 1];
          // not perfect, but close enough for first version
          if ( f == 'x' || f == 'X' || f == 'u' || f == 'U' ) {
            while ( (start + len) < text.Length && IsHexDigit(text[start + len + 1]) ) {
              len++;
            }
          }
          if ( f == 'u' ) maxlen = 5;
          if ( f == 'U' ) maxlen = 9;
          if ( len > maxlen ) len = maxlen;
          var sspan = new SnapshotSpan(cs.Snapshot, cs.Start.Position + start, len + 1);
          yield return new TagSpan<KeywordTag>(sspan, stringEscapeClassification);
          start += len;
        }
        start++;
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
