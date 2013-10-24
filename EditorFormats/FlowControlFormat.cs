using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.KEYWORD_CLASSIF_NAME)]
  [Name(Constants.KEYWORD_CLASSIF_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class FlowControlFormat : ClassificationFormatDefinition {
    public FlowControlFormat() {
      this.DisplayName = Constants.KEYWORD_CLASSIF_NAME;
      this.ForegroundColor = Colors.OrangeRed;
      this.IsItalic = true;
    }
  }
}
