using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.LANGAUGESERVICE_METHOD_OVERLOAD_NAME)]
  [Name(Constants.LANGAUGESERVICE_METHOD_OVERLOAD_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class MethodOverloadFormat : ClassificationFormatDefinition {
    public MethodOverloadFormat() {
      this.DisplayName = "Viasfora Method Overload";
      this.ForegroundOpacity = 0.5;
    }
  }
}
