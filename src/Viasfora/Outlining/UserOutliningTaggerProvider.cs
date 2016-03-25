using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Outlining {
  [Export(typeof(ITaggerProvider))]
  [ContentType(ContentTypes.Text)]
  [TagType(typeof(IOutliningRegionTag))]
  [TagType(typeof(IGlyphTag))]
  [TextViewRole(PredefinedTextViewRoles.Structured)]
  public class UserOutliningTaggerProvider : ITaggerProvider {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      IOutliningManager manager = UserOutliningManager.GetManager(buffer);
      if ( typeof(T) == typeof(IOutliningRegionTag) ) {
        return manager.GetOutliningTagger() as ITagger<T>;
      }
      return manager.GetGlyphTagger() as ITagger<T>;
    }
  }
}
