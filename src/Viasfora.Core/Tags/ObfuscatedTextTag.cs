using System;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Classification;

namespace Winterdom.Viasfora.Tags {
  public class ObfuscatedTextTag : IClassificationTag {
    public IClassificationType ClassificationType { get; private set; }
    public ObfuscatedTextTag(IClassificationType classification) {
      this.ClassificationType = classification;
    }
  }
}
