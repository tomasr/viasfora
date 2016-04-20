using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {

  class XmlTagger : ITagger<ClassificationTag>, IDisposable {
    private ITextBuffer theBuffer;
    private IXmlSettings settings;
    private ClassificationTag xmlCloseTagClassification;
    private ClassificationTag xmlPrefixClassification;
    private ClassificationTag xmlClosingPrefixClassification;
    private ClassificationTag xmlDelimiterClassification;
    private ClassificationTag razorCloseTagClassification;
    private IMarkupLanguage language;
    private ITagAggregator<IClassificationTag> aggregator;
    private static readonly List<ITagSpan<ClassificationTag>> EmptyList =
      new List<ITagSpan<ClassificationTag>>();

#pragma warning disable 67
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

    internal XmlTagger(
        ITextBuffer buffer,
        IClassificationTypeRegistryService registry,
        ITagAggregator<IClassificationTag> aggregator,
        IXmlSettings settings) {
      this.theBuffer = buffer;
      this.settings = settings;
      xmlCloseTagClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.XML_CLOSING));
      xmlPrefixClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.XML_PREFIX));
      xmlClosingPrefixClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.XML_CLOSING_PREFIX));
      xmlDelimiterClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.DELIMITER));
      razorCloseTagClassification =
         new ClassificationTag(registry.GetClassificationType(Constants.RAZOR_CLOSING));
      settings.SettingsChanged += OnSettingsChanged;
      this.aggregator = aggregator;
    }

    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count > 0 ) {
        ITextSnapshot snapshot = spans[0].Snapshot;
        IContentType fileType = snapshot.TextBuffer.ContentType;
        if ( fileType.IsOfType(Constants.CT_XML) ) {
          if ( language == null ) language = new XmlMarkup();
          return DoXML(spans);
        } else if ( fileType.IsOfType(Constants.CT_XAML) ) {
          if ( language == null ) language = new XamlMarkup();
          return DoXAMLorHTML(spans);
        } else if ( fileType.IsOfType(Constants.CT_HTML) 
                 || fileType.IsOfType(Constants.CT_HTMLX) ) {
          if ( language == null ) language = new HtmlMarkup();
          return DoXAMLorHTML(spans);
        }
      }
      return EmptyList;
    }

    public void Dispose() {
      if ( this.settings != null ) {
        this.settings.SettingsChanged -= OnSettingsChanged;
        this.settings = null;
      }
      theBuffer = null;
    }
    void OnSettingsChanged(object sender, EventArgs e) {
      var tempEvent = TagsChanged;
      if ( tempEvent != null ) {
        tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(theBuffer.CurrentSnapshot, 0,
            theBuffer.CurrentSnapshot.Length)));
      }
    }

    private IEnumerable<ITagSpan<ClassificationTag>> DoXML(NormalizedSnapshotSpanCollection spans) {
      ITextSnapshot snapshot = spans[0].Snapshot;
      bool foundClosingTag = false;

      foreach ( var tagSpan in aggregator.GetTags(spans) ) {
        String tagName = tagSpan.Tag.ClassificationType.Classification;
        var cs = tagSpan.Span.GetSpans(snapshot)[0];
        if ( IsXmlDelimiter(tagName) ) {
          if ( cs.GetText().EndsWith("</") ) {
            foundClosingTag = true;
          }
        } else if ( IsXmlName(tagName) || IsXmlAttribute(tagName) ) {
          foreach ( var ct in ProcessXmlName(cs, foundClosingTag) ) {
            yield return ct;
          }
          foundClosingTag = false;
        }
      }
    }

    private IEnumerable<ITagSpan<ClassificationTag>> DoXAMLorHTML(NormalizedSnapshotSpanCollection spans) {
      ITextSnapshot snapshot = spans[0].Snapshot;
      bool foundClosingTag = false;
      SnapshotSpan? lastSpan = null;
      String lastSpanTagName = null;

      // XAML parses stuff in really weird ways, so we need to special case it
      foreach ( var tagSpan in aggregator.GetTags(spans) ) {
        String tagName = tagSpan.Tag.ClassificationType.Classification;
        var cs = tagSpan.Span.GetSpans(snapshot)[0];
        if ( IsXmlDelimiter(tagName) ) {
          String text = cs.GetText();
          if ( text.EndsWith("</") ) {
            foundClosingTag = true;
          } else if ( text == ":" && lastSpan.HasValue && settings.XmlnsPrefixEnabled ) {
            var prefixCT = foundClosingTag && settings.XmlCloseTagEnabled
                         ? xmlClosingPrefixClassification
                         : xmlPrefixClassification;
            yield return new TagSpan<ClassificationTag>(lastSpan.Value, prefixCT);
          } else if ( text.IndexOf('>') >= 0 && lastSpan.HasValue && foundClosingTag && settings.XmlCloseTagEnabled ) {
            yield return new TagSpan<ClassificationTag>(lastSpan.Value, xmlCloseTagClassification);
            foundClosingTag = false;
          }
          lastSpan = null;
          lastSpanTagName = null;
        } else if ( IsXmlName(tagName) || IsXmlAttribute(tagName) ) {
          lastSpan = cs;
          lastSpanTagName = tagName;
        } else if ( IsRazorTag(tagName) ) {
          if ( snapshot.GetText(new Span(cs.Span.Start - 2, 2)) == "</" ) {
            yield return new TagSpan<ClassificationTag>(cs, razorCloseTagClassification);
          }
        }
      }
    }

    private IEnumerable<ITagSpan<ClassificationTag>> ProcessXmlName(SnapshotSpan cs, bool isClosing) {
      String text = cs.GetText();
      int colon = text.IndexOf(':');
      if ( colon < 0 && isClosing && settings.XmlCloseTagEnabled ) {
        yield return new TagSpan<ClassificationTag>(cs, xmlCloseTagClassification);
      } else if ( colon > 0 && settings.XmlnsPrefixEnabled ) {
        string prefix = text.Substring(0, colon);
        string name = text.Substring(colon + 1);

        var prefixCT = isClosing && settings.XmlCloseTagEnabled
                     ? xmlClosingPrefixClassification
                     : xmlPrefixClassification;
        yield return new TagSpan<ClassificationTag>(
          new SnapshotSpan(cs.Start, prefix.Length), prefixCT);

        yield return new TagSpan<ClassificationTag>(new SnapshotSpan(
          cs.Start.Add(prefix.Length), 1), xmlDelimiterClassification);

        if ( isClosing && settings.XmlCloseTagEnabled ) {
          yield return new TagSpan<ClassificationTag>(new SnapshotSpan(
            cs.Start.Add(prefix.Length + 1), name.Length), xmlCloseTagClassification);
        }
      } else if ( isClosing && settings.XmlCloseTagEnabled ) {
        // XmlnsPrefix hl disabled, but we still want to highlight the closing tag
        yield return new TagSpan<ClassificationTag>(cs, xmlCloseTagClassification);
      }
    }

    private bool IsXmlName(String tagName) {
      return language.IsName(tagName);
    }

    private bool IsXmlAttribute(String tagName) {
      return language.IsAttribute(tagName);
    }

    private bool IsXmlDelimiter(String tagName) {
      return language.IsDelimiter(tagName);
    }

    private bool IsRazorTag(String tagName) {
      return language.IsRazorTag(tagName);
    }
  }
}
