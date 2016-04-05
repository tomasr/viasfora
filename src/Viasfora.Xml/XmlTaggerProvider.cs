using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {
  [Export(typeof(IViewTaggerProvider))]
  [ContentType(Constants.CT_XML)]
  [ContentType(Constants.CT_XAML)]
  [ContentType(Constants.CT_HTML)]
  [ContentType(Constants.CT_HTMLX)]
  [TagType(typeof(ClassificationTag))]
  public class XmlTaggerProvider : IViewTaggerProvider {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry { get; set; }
    [Import]
    internal IBufferTagAggregatorFactoryService Aggregator { get; set; }
    [Import]
    internal IXmlSettings Settings { get; set; }

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
      return new XmlTagger(
         buffer,
         ClassificationRegistry,
         Aggregator.CreateTagAggregator<IClassificationTag>(buffer),
         Settings
      ) as ITagger<T>;
    }
  }
}
