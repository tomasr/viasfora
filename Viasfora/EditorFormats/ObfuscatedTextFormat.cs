using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.OBFUSCATED_TEXT)]
  [Name(Constants.OBFUSCATED_TEXT)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class ObfuscatedTextFormatDefinition : ClassificationFormatDefinition {
    public ObfuscatedTextFormatDefinition() {
      this.DisplayName = "Viasfora Obfuscated Text";
      this.ForegroundColor = Colors.Transparent;
      this.ForegroundOpacity = 0;
      this.BackgroundOpacity = 0.7;
      this.BackgroundColor = Colors.Linen;
      this.ForegroundCustomizable = false;
    }
  }
}
