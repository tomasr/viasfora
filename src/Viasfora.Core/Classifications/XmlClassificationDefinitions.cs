using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Classifications {

  internal static class XmlClassificationDefinitions {
    [Export, Name(Constants.XML_CLOSING)]
    internal static ClassificationTypeDefinition XmlClosingType = null;

    [Export, Name(Constants.XML_PREFIX)]
    internal static ClassificationTypeDefinition XmlPrefixType = null;

    [Export, Name(Constants.RAZOR_CLOSING)]
    internal static ClassificationTypeDefinition RazorClosingType = null;
  }
}
