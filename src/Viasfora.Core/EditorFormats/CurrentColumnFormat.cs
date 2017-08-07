using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.COLUMN_HIGHLIGHT)]
  [Name(Constants.COLUMN_HIGHLIGHT)]
  [UserVisible(true)]
  [Order(Before = Priority.Default)]
  sealed class CurrentColumnFormat : ClassificationFormatDefinition {
    public CurrentColumnFormat() {
      this.DisplayName = "Viasfora Current Column";
      this.ForegroundColor = Colors.LightGray;
      this.ForegroundOpacity = 0.3;
      this.BackgroundOpacity = 0.3;
    }
  }
}
