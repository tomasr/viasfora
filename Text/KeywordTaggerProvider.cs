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
  [ContentType(CSharp.ContentType)]
  [ContentType(Cpp.ContentType)]
  [ContentType(VB.ContentType)]
  [ContentType(JScript.ContentType)]
  [ContentType(JScript.ContentTypeVS2012)]
  [ContentType(FSharp.ContentType)]
  [TagType(typeof(ClassificationTag))]
  public class KeywordTaggerProvider : IViewTaggerProvider {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null;
    [Import]
    internal IBufferTagAggregatorFactoryService Aggregator = null;

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
      return new KeywordTagger(
         buffer,
         ClassificationRegistry,
         Aggregator.CreateTagAggregator<IClassificationTag>(buffer)
      ) as ITagger<T>;
    }
  }

}
