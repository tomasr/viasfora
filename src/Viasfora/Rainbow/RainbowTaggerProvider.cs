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
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Rainbow {

  [Export(typeof(ITaggerProvider))]
  [ContentType(ContentTypes.Text)]
  [TagType(typeof(RainbowTag))]
  public class RainbowTaggerProvider : ITaggerProvider {
    [Import]
    public IClassificationTypeRegistryService ClassificationRegistry { get; set; }
    [Import]
    public ILanguageFactory LanguageFactory { get; set; }
    [Import]
    public IVsfSettings Settings { get; set; }

    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      RainbowProvider prov = buffer.Properties.GetOrCreateSingletonProperty(
        () => new RainbowProvider(buffer, this)
        );
      return prov.ColorTagger as ITagger<T>;
    }
  }
}
