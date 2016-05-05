using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Classifications {

  internal static class XmlClassificationDefinitions {
    [Export, Name(XmlConstants.XML_CLOSING)]
    internal static ClassificationTypeDefinition XmlClosingType = null;

    [Export, Name(XmlConstants.XML_PREFIX)]
    internal static ClassificationTypeDefinition XmlPrefixType = null;

    [Export, Name(XmlConstants.XML_CLOSING_PREFIX)]
    internal static ClassificationTypeDefinition XmlClosingPrefixType = null;

    [Export, Name(XmlConstants.RAZOR_CLOSING)]
    internal static ClassificationTypeDefinition RazorClosingType = null;
  }
}
