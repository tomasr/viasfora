using System;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Classification;

namespace Winterdom.Viasfora.Tags {
  public class RainbowTag : IClassificationTag {
    public IClassificationType ClassificationType { get; private set; }
    public RainbowTag(IClassificationType classification) {
      this.ClassificationType = classification;
    }
  }

}
