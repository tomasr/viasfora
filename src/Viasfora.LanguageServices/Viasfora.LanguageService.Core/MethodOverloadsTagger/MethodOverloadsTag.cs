using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using System;

namespace Winterdom.Viasfora.LanguageService.Core.MethodOverloadsTagger {
  public class MethodOverloadsTag : IClassificationTag {

    public IClassificationType ClassificationType { get; private set; }

    public MethodOverloadsTag(IClassificationType classification) {
      this.ClassificationType = classification ?? throw new ArgumentNullException(nameof(classification));
    }
  }
}
