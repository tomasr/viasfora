using Microsoft.VisualStudio.Language.StandardClassification;
using System;

namespace Winterdom.Viasfora {
  public static class XmlConstants {
    public const String CT_XML = "XML";
    public const String CT_XAML = "XAML";
    public const String CT_HTML = "HTML";

    // VS213 HTML Editor
    public const String CT_HTMLX = "htmlx";
    public const String XML_CLOSING = "viasfora.xml.closing";
    public const String XML_PREFIX = "viasfora.xml.prefix";
    public const String XML_CLOSING_PREFIX = "viasfora.xml.closing.prefix";
    public const String RAZOR_CLOSING = "viasfora.razor.closing.element";
    // I'd prefer "XML Delimiter" here, but no way to
    // use it effectively.
    public const String DELIMITER = PredefinedClassificationTypeNames.Operator;

  }
}
