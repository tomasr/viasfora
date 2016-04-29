using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.XML_CLOSING_PREFIX)]
  [Name(Constants.XML_CLOSING_PREFIX)]
  [UserVisible(true)]
  [Order(Before = Priority.High, After = Priority.High)]
  internal sealed class XmlClosingPrefixFormat : ClassificationFormatDefinition {
    public XmlClosingPrefixFormat() {
      this.DisplayName = "Viasfora XML Prefix Closing";
      this.ForegroundColor = Colors.OrangeRed;
    }
  }

}
