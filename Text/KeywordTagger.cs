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
      LanguageInfo lang =
         GetKeywordsByContentType(spans[0].Snapshot.TextBuffer.ContentType);
      bool eshe = VsfSettings.EscapeSeqHighlightEnabled && lang.SupportsEscapeSeqs;
      bool kce = VsfSettings.KeywordClassifierEnabled;

      if ( kce || eshe ) {
        foreach ( var tag in GetClassifiedSpans(spans, "string", "keyword") ) {
          if ( eshe && tag.Item1 == "string" ) {
            // Extract escape sequences from string
            foreach ( var escapeTag in ProcessEscapeSequences(tag.Item2) ) {
              yield return escapeTag;
            }
          } else if ( kce && tag.Item1 == "keyword" ) {
            // Is this one of the keywords we care about?
            var result = IsInterestingKeyword(lang, tag.Item2);
            if ( result != null ) {
              yield return result;
            }
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
      String text = cs.GetText().ToLower();
      if ( lang.ControlFlow.Contains(text) ) {
        return new TagSpan<KeywordTag>(cs, keywordClassification);
      } else if ( lang.Visibility.Contains(text) ) {
        return new TagSpan<KeywordTag>(cs, visClassification);
      } else if ( lang.Linq.Contains(text) ) {
        return new TagSpan<KeywordTag>(cs, linqClassification);
      }
      return null;
    }

    private IEnumerable<ITagSpan<KeywordTag>> ProcessEscapeSequences(SnapshotSpan cs) {
      String text = cs.GetText();
      // don't process verbatim strings
      if ( text.StartsWith("@") ) yield break;
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

    private bool HasMatchingName(String[] wanted, String actual, out String match) {
      match = null;
      foreach ( String w in wanted ) {
        if ( actual.Contains(w) ) {
          match = w;
          return true;
        }
      }
      return false;
    }

    private IEnumerable<Tuple<String, SnapshotSpan>> GetClassifiedSpans(
          NormalizedSnapshotSpanCollection spans, params String[] tagNames) {
      ITextSnapshot snapshot = spans[0].Snapshot;
      foreach ( var tagSpan in aggregator.GetTags(spans) ) {
        String name = tagSpan.Tag.ClassificationType.Classification.ToLower();
        String matchingName;
        if ( HasMatchingName(tagNames, name, out matchingName) ) {
          var mappedSpans = tagSpan.Span.GetSpans(snapshot);
          if ( mappedSpans.Count > 0 ) {
            yield return new Tuple<String, SnapshotSpan>(matchingName, mappedSpans[0]);
          }
        }
      }
    }

    private LanguageInfo GetKeywordsByContentType(IContentType contentType) {
      return VsfPackage.LookupLanguage(contentType);
    }
  }

}
