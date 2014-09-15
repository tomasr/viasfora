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
  [ContentType("Text")]
  [TagType(typeof(IOutliningRegionTag))]
  [TextViewRole(PredefinedTextViewRoles.Structured)]
  public class FeatureOutliningTaggerProvider : ITaggerProvider {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      IOutliningManager manager = FeatureOutliningManager.GetManager(buffer);
      if ( typeof(T) == typeof(IOutliningRegionTag) ) {
        return manager.GetOutliningTagger() as ITagger<T>;
      }
      return null;
    }
  }
}
