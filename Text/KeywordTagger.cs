using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  class KeywordTagger : ITagger<ClassificationTag> {
    private ClassificationTag keywordClassification;
    private ClassificationTag linqClassification;
    private ClassificationTag visClassification;
    private ClassificationTag stringEscapeClassification;
    private ITagAggregator<IClassificationTag> aggregator;
    private static readonly IList<ClassificationSpan> EmptyList =
       new List<ClassificationSpan>();

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal KeywordTagger(
          IClassificationTypeRegistryService registry,
          ITagAggregator<IClassificationTag> aggregator) {
      keywordClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.KEYWORD_CLASSIF_NAME));
      linqClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.LINQ_CLASSIF_NAME));
      visClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.VISIBILITY_CLASSIF_NAME));
      stringEscapeClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.STRING_ESCAPE_CLASSIF_NAME));
      this.aggregator = aggregator;
    }

    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        yield break;
      }
      if ( VsfSettings.EscapeSeqHighlightEnabled ) {
        foreach ( var tagSpan in LookForStringEscapeSequences(spans) ) {
          yield return tagSpan;
        }
      }
      if ( VsfSettings.KeywordClassifierEnabled ) {
        foreach ( var tagSpan in LookForKeywords(spans) ) {
          yield return tagSpan;
        }
      }
    }



    private IEnumerable<ITagSpan<ClassificationTag>> LookForKeywords(NormalizedSnapshotSpanCollection spans) {
      ITextSnapshot snapshot = spans[0].Snapshot;
      LanguageKeywords keywords =
         GetKeywordsByContentType(snapshot.TextBuffer.ContentType);
      if ( keywords == null ) {
        yield break;
      }

      // find spans that the language service has already classified as keywords ...
      var classifiedSpans = GetClassifiedSpans(spans, "keyword");

      // ... and from those, ones that match our keywords
      foreach ( var cs in classifiedSpans ) {
        String text = cs.GetText();
        if ( keywords.ControlFlow.Contains(text) ) {
          yield return new TagSpan<ClassificationTag>(cs, keywordClassification);
        } else if ( keywords.Visibility.Contains(text) ) {
          yield return new TagSpan<ClassificationTag>(cs, visClassification);
        } else if ( keywords.Linq.Contains(text) ) {
          yield return new TagSpan<ClassificationTag>(cs, linqClassification);
        }
      }
    }

    private IEnumerable<ITagSpan<ClassificationTag>> LookForStringEscapeSequences(NormalizedSnapshotSpanCollection spans) {
      ITextSnapshot snapshot = spans[0].Snapshot;
      var classifiedSpans = GetClassifiedSpans(spans, "string");

      foreach ( var cs in classifiedSpans ) {
        String text = cs.GetText();
        // don't process verbatim strings
        if ( text.StartsWith("@") ) continue;
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
            if ( f == 'u' ) maxlen = 4;
            if ( f == 'U' ) maxlen = 8;
            if ( len > maxlen ) len = maxlen;
            var sspan = new SnapshotSpan(snapshot, cs.Start.Position + start, len + 1);
            yield return new TagSpan<ClassificationTag>(sspan, stringEscapeClassification);
            start += len;
          }
          start++;
        }
      }
    }

    private bool IsHexDigit(char c) {
      if ( Char.IsDigit(c) ) return true;
      return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }

    private IEnumerable<SnapshotSpan> GetClassifiedSpans(NormalizedSnapshotSpanCollection spans, String tagName) {
      ITextSnapshot snapshot = spans[0].Snapshot;
      var mappedSpans =
         from tagSpan in aggregator.GetTags(spans)
         let name = tagSpan.Tag.ClassificationType.Classification.ToLower()
         where name.Contains(tagName)
         select tagSpan.Span;
      var classifiedSpans =
         from mappedSpan in mappedSpans
         let cs = mappedSpan.GetSpans(snapshot)
         where cs.Count > 0
         select cs[0];

      return classifiedSpans;
    }

    private LanguageKeywords GetKeywordsByContentType(IContentType contentType) {
      if ( contentType.IsOfType(CSharp.ContentType) ) {
        return new CSharp();
      } else if ( contentType.IsOfType(Cpp.ContentType) ) {
        return new Cpp();
      } else if ( contentType.IsOfType(JScript.ContentType)
               || contentType.IsOfType(JScript.ContentTypeVS2012) ) {
        return new JScript();
      }
      // VS is calling us for the "CSharp Signature Help" content-type
      // which we didn't ask for. Argh!!!
      // throw new InvalidOperationException("Running into an unsupported editor");
      return null;
    }
  }

}
