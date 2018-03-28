using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Tags;
using Winterdom.Viasfora.Languages;

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
    public IRainbowSettings Settings { get; set; }

    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      RainbowProvider prov = buffer.Properties.GetOrCreateSingletonProperty(
        () => new RainbowProvider(buffer, this)
        );
      return prov.ColorTagger as ITagger<T>;
    }
  }
}
