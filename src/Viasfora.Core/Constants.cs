using System;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace Winterdom.Viasfora {
  public static class Constants {
    public const String KEYWORD_CLASSIF_NAME = "Viasfora Flow Control Keyword";
    public const String LINQ_CLASSIF_NAME = "Viasfora Query Operator";
    public const String VISIBILITY_CLASSIF_NAME = "Viasfora Visibility Keyword";
    public const String STRING_ESCAPE_CLASSIF_NAME = "Viasfora String Escape Sequence";
    public const String FORMAT_SPECIFIER_NAME = "viasfora.format.specifier";
    public const String LINE_HIGHLIGHT = "Viasfora Current Line";
    public const String COLUMN_HIGHLIGHT = "Viasfora Current Column";
    public const String DEV_MARGIN = "viasfora.dev.margin";

    public const String OBFUSCATED_TEXT = "viasfora.text.obfuscated";

    public const String CT_XML = "XML";
    public const String CT_XAML = "XAML";
    public const String CT_HTML = "HTML";
    // VS213 HTML Editor
    public const String CT_HTMLX = "htmlx";
    public const String XML_CLOSING = "XMLCloseTag";
    public const String XML_PREFIX = "XMLPrefix";
    public const String RAZOR_CLOSING = "viasfora.razor.closing.element";
    // I'd prefer "XML Delimiter" here, but no way to
    // use it effectively.
    public const String DELIMITER = PredefinedClassificationTypeNames.Operator;

    public const String STRING_COLLECTION_EDITOR = "System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
    public static int S_OK = 0;

    // Languages
    public const String Cpp = "Cpp";
    public const String CSharp = "CSharp";
    public const String XLang = "XLANG/s";
    public const String Css = "CSS";
    public const String FSharp = "FSharp";
    public const String JS = "JScript";
    public const String Json = "JSON";
    public const String PowerShell = "PowerShell";
    public const String Python = "Python";
    public const String R = "R";
    public const String Sql = "Sql";
    public const String TypeScript = "TypeScript";
    public const String VB = "VB";
  }
}
