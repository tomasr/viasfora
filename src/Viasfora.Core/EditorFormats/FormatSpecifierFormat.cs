using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.FORMAT_SPECIFIER_NAME)]
  [Name(Constants.FORMAT_SPECIFIER_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class FormatSpecifierFormat : ClassificationFormatDefinition {
    public FormatSpecifierFormat() {
      this.DisplayName = "Viasfora Format Specifier";
      this.ForegroundColor = Colors.MediumSlateBlue;
    }
  }
}
