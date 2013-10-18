using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.XML_CLOSING)]
  [Name(Constants.XML_CLOSING)]
  [UserVisible(true)]
  [Order(Before = Priority.High, After = Priority.High)]
  internal sealed class XmlClosingTagFormat : ClassificationFormatDefinition {
    public XmlClosingTagFormat() {
      this.DisplayName = "XML Closing Tag";
      this.ForegroundColor = Colors.MediumVioletRed;
    }
  }

}
