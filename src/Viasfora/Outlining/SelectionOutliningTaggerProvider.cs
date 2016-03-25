using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Outlining {
  [Export(typeof(ITaggerProvider))]
  [ContentType(ContentTypes.Any)]
  [ContentType(ContentTypes.Projection)]
  [TagType(typeof(IOutliningRegionTag))]
  [TextViewRole(PredefinedTextViewRoles.Structured)]
  public class SelectionOutliningTaggerProvider : ITaggerProvider {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      IOutliningManager manager = SelectionOutliningManager.GetManager(buffer);
      if ( typeof(T) == typeof(IOutliningRegionTag) ) {
        return manager.GetOutliningTagger() as ITagger<T>;
      }
      return null;
    }
  }
}
