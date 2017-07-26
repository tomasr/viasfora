using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.LINQ_CLASSIF_NAME)]
  [Name(Constants.LINQ_CLASSIF_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class LinqKeywordFormat : ClassificationFormatDefinition {
    public LinqKeywordFormat() {
      this.DisplayName = "Viasfora Query Operator";
      this.ForegroundColor = Colors.MediumSeaGreen;
    }
  }
}
