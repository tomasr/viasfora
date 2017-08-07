using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.FLOW_CONTROL_CLASSIF_NAME)]
  [Name(Constants.FLOW_CONTROL_CLASSIF_NAME)]
  [UserVisible(true)]
  [Order(After = Priority.High)]
  public sealed class FlowControlFormat : ClassificationFormatDefinition {
    public FlowControlFormat() {
      this.DisplayName = "Viasfora Flow Control Keyword";
      this.ForegroundColor = Colors.OrangeRed;
    }
  }
}
