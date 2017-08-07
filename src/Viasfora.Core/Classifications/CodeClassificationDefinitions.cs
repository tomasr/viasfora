using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Classifications {

  public static class CodeClassificationDefinitions {
    [Export, Name(Constants.FLOW_CONTROL_CLASSIF_NAME)]
    internal static ClassificationTypeDefinition FlowControlClassificationType = null;

    [Export, Name(Constants.LINQ_CLASSIF_NAME)]
    internal static ClassificationTypeDefinition LinqKeywordClassificationType = null;

    [Export, Name(Constants.STRING_ESCAPE_CLASSIF_NAME)]
    internal static ClassificationTypeDefinition StringEscapeSequenceClassificationType = null;

    [Export, Name(Constants.FORMAT_SPECIFIER_NAME)]
    internal static ClassificationTypeDefinition FormatSpecifierClassificationType = null;

    [Export, Name(Constants.VISIBILITY_CLASSIF_NAME)]
    internal static ClassificationTypeDefinition VisibilityKeywordClassificationType = null;

    [Export, Name(Constants.LINE_HIGHLIGHT)]
    internal static ClassificationTypeDefinition CurrentLineClassificationType = null;

    [Export, Name(Constants.COLUMN_HIGHLIGHT)]
    internal static ClassificationTypeDefinition CurrentColumnClassificationType = null;

    [Export, Name(Constants.OBFUSCATED_TEXT)]
    internal static ClassificationTypeDefinition ObfuscatedTextType = null;
  } 
}
