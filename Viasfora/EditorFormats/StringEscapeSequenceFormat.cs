using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.STRING_ESCAPE_CLASSIF_NAME)]
  [Name(Constants.STRING_ESCAPE_CLASSIF_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class StringEscapeSequenceFormat : ClassificationFormatDefinition {
    public StringEscapeSequenceFormat() {
      this.DisplayName = Constants.STRING_ESCAPE_CLASSIF_NAME;
      this.ForegroundColor = Colors.DimGray;
    }
  }
}
