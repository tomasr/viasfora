using System;
using System.ComponentModel.Composition;
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
