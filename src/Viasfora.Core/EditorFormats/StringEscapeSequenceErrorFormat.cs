using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.STRING_ESCAPE_ERROR_NAME)]
  [Name(Constants.STRING_ESCAPE_ERROR_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class StringEscapeSequenceErrorFormat : ClassificationFormatDefinition {
    public StringEscapeSequenceErrorFormat() {
      this.DisplayName = "Viasfora Invalid String Escape Sequence";
      this.ForegroundColor = Color.FromRgb(255, 160, 0);
    }
  }
}
