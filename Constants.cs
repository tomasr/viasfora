using System;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace Winterdom.Viasfora {
  public static class Constants {
    public const String KEYWORD_CLASSIF_NAME = "Keyword - Flow Control";
    public const String LINQ_CLASSIF_NAME = "Operator - LINQ";
    public const String VISIBILITY_CLASSIF_NAME = "Keyword - Visibility";
    public const String STRING_ESCAPE_CLASSIF_NAME = "String Escape Sequence";
    public const String LINE_HIGHLIGHT = "Current Line";

    public const string CT_XML = "XML";
    public const string CT_XAML = "XAML";
    public const string CT_HTML = "HTML";
    public const string XML_CLOSING = "XMLCloseTag";
    public const string XML_PREFIX = "XMLPrefix";
    // I'd prefer "XML Delimiter" here, but no way to
    // use it effectively.
    public const string DELIMITER = PredefinedClassificationTypeNames.Operator;
  }
}
