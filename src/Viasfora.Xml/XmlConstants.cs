using Microsoft.VisualStudio.Language.StandardClassification;
using System;

namespace Winterdom.Viasfora {
  public static class XmlConstants {
    public const String CT_XML = "XML";
    public const String CT_XAML = "XAML";
    public const String CT_HTML = "HTML";

    // VS213 HTML Editor
    public const String CT_HTMLX = "htmlx";
    public const String XML_CLOSING = "XMLCloseTag";
    public const String XML_PREFIX = "XMLPrefix";
    public const String XML_CLOSING_PREFIX = "viasfora.xml.closing.prefix";
    public const String RAZOR_CLOSING = "viasfora.razor.closing.element";
    // I'd prefer "XML Delimiter" here, but no way to
    // use it effectively.
    public const String DELIMITER = PredefinedClassificationTypeNames.Operator;

  }
}
