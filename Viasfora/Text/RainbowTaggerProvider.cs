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

  [Export(typeof(IViewTaggerProvider))]
  [ContentType(CSharp.ContentType)]
  [ContentType(Cpp.ContentType)]
  [ContentType(JScript.ContentType)]
  [ContentType(JScript.ContentTypeVS2012)]
  [ContentType(JSON.ContentType)]
  [ContentType(VB.ContentType)]
  [ContentType(FSharp.ContentType)]
  [ContentType(Sql.ContentType)]
  [ContentType(Sql.ContentTypeAlt)]
  [ContentType(TypeScript.ContentType)]
  [ContentType(Python.ContentType)]
  [ContentType(PowerShell.ContentType)]
  [TagType(typeof(RainbowTag))]
  public class RainbowTaggerProvider : IViewTaggerProvider {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null;

    public ITagger<T> CreateTagger<T>(ITextView view, ITextBuffer buffer) where T : ITag {
      // for a complex editor such as the HTMLX editor in VS2013
      // we need to have multiple of these created, for different text buffers
      // so this cannot be a singleton property, but we need to shim it based on
      // the buffer, not the view
      RainbowProvider prov = buffer.Properties.GetOrCreateSingletonProperty(
        () => new RainbowProvider(view, buffer, ClassificationRegistry)
        );
      return prov.ColorTagger as ITagger<T>;
    }
  }
}
