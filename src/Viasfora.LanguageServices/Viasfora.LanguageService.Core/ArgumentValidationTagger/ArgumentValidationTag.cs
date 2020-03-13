using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Viasfora.LanguageService.Core.ArgumentValidationTagger {
  public class ArgumentValidationTag : IClassificationTag {

    public IClassificationType ClassificationType { get; private set; }
  
    public ArgumentValidationTag(IClassificationType classification) {
      this.ClassificationType = classification ?? throw new ArgumentNullException(nameof(classification));
    }
  }
}
