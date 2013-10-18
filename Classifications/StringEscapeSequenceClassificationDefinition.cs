using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Classifications {
  public static class StringEscapeSequenceClassificationDefinition {
    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Constants.STRING_ESCAPE_CLASSIF_NAME)]
    internal static ClassificationTypeDefinition StringEscapeSequenceClassificationType = null;
  }
}
