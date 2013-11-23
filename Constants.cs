using System;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace Winterdom.Viasfora {
  public static class Constants {
    public const String KEYWORD_CLASSIF_NAME = "Keyword - Flow Control";
    public const String LINQ_CLASSIF_NAME = "Operator - LINQ";
    public const String VISIBILITY_CLASSIF_NAME = "Keyword - Visibility";
    public const String STRING_ESCAPE_CLASSIF_NAME = "String Escape Sequence";
    public const String LINE_HIGHLIGHT = "Current Line";

    public const String RAINBOW = "Rainbow Parentheses ";
    public const String RAINBOW_1 = "Rainbow Parentheses 1";
    public const String RAINBOW_2 = "Rainbow Parentheses 2";
    public const String RAINBOW_3 = "Rainbow Parentheses 3";
    public const String RAINBOW_4 = "Rainbow Parentheses 4";
    public const String RAINBOW_5 = "Rainbow Parentheses 5";
    public const String RAINBOW_6 = "Rainbow Parentheses 6";
    public const String RAINBOW_7 = "Rainbow Parentheses 7";
    public const String RAINBOW_8 = "Rainbow Parentheses 8";
    public const String RAINBOW_9 = "Rainbow Parentheses 9";

    public const String CT_XML = "XML";
    public const String CT_XAML = "XAML";
    public const String CT_HTML = "HTML";
    // VS213 HTML Editor
    public const String CT_HTMLX = "htmlx";
    public const String XML_CLOSING = "XMLCloseTag";
    public const String XML_PREFIX = "XMLPrefix";
    // I'd prefer "XML Delimiter" here, but no way to
    // use it effectively.
    public const String DELIMITER = PredefinedClassificationTypeNames.Operator;

    public const String STRING_COLLECTION_EDITOR = "System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
  }
}
