using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.LANGUAGESERVICE_ARGUMENT_VALIDATION_NAME)]
  [Name(Constants.LANGUAGESERVICE_ARGUMENT_VALIDATION_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class ArgumentValidationFormat : ClassificationFormatDefinition {
    public ArgumentValidationFormat() {
      this.DisplayName = "Viasfora Argument Validation";
      this.ForegroundOpacity = 0.5;
    }
  }
}
