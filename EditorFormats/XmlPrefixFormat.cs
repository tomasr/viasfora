using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.EditorFormats {

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.XML_PREFIX)]
  [Name(Constants.XML_PREFIX)]
  [UserVisible(true)]
  [Order(Before = Priority.High, After = Priority.High)]
  internal sealed class XmlPrefixFormat : ClassificationFormatDefinition {
    public XmlPrefixFormat() {
      this.DisplayName = "XML Prefix";
      this.ForegroundColor = Colors.ForestGreen;
    }
  }

}
