using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.VISIBILITY_CLASSIF_NAME)]
  [Name(Constants.VISIBILITY_CLASSIF_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class VisibilityKeywordFormat : ClassificationFormatDefinition {
    public VisibilityKeywordFormat() {
      this.DisplayName = Constants.VISIBILITY_CLASSIF_NAME;
      this.ForegroundColor = Colors.DimGray;
      this.IsBold = true;
    }
  }
}
