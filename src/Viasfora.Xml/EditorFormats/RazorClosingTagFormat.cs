using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = XmlConstants.RAZOR_CLOSING)]
  [Name(XmlConstants.RAZOR_CLOSING)]
  [UserVisible(true)]
  [Order(Before = Priority.High, After = Priority.High)]
  internal sealed class RazorClosingTagFormat : ClassificationFormatDefinition {
    public RazorClosingTagFormat() {
      this.DisplayName = "Viasfora Razor Tag Closing Element";
      this.ForegroundColor = Colors.DarkOrange;
      this.IsBold = true;
    }
  }

}
