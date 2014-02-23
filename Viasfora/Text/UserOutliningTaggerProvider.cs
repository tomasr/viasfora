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

namespace Winterdom.Viasfora.Text {
  [Export(typeof(ITaggerProvider))]
  [ContentType("Text")]
  [TagType(typeof(IOutliningRegionTag))]
  [TextViewRole(PredefinedTextViewRoles.Structured)]
  public class UserOutliningTaggerProvider : ITaggerProvider {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      IOutliningManager manager = OutliningManager.GetManager(buffer);
      return manager.GetOutliningTagger() as ITagger<T>;
    }
  }
}
