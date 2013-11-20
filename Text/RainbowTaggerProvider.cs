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

  [Export(typeof(IViewTaggerProvider))]
  [ContentType("Text")]
  [TagType(typeof(TextMarkerTag))]
  public class RainbowTaggerProvider : IViewTaggerProvider {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null;
    [Import]
    internal IBufferTagAggregatorFactoryService Aggregator = null;

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
      return new RainbowTagger(buffer, textView) as ITagger<T>;
    }
  }

}
