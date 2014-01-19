using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(ITaggerProvider))]
  [ContentType("Text")]
  [TagType(typeof(IOutliningRegionTag))]
  public class UserRegionTaggerProvider : ITaggerProvider {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      return new UserRegionTagger(buffer) as ITagger<T>;
    }
  }
}
