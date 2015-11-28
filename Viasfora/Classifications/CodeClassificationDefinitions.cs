using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Classifications {

  internal static class CodeClassificationDefinitions {
    [Export, Name(Constants.KEYWORD_CLASSIF_NAME)]
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

    [Export, Name(Constants.RAINBOW_1)]
    internal static ClassificationTypeDefinition Rainbow1ClassificationType = null;

    [Export, Name(Constants.RAINBOW_2)]
    internal static ClassificationTypeDefinition Rainbow2ClassificationType = null;

    [Export, Name(Constants.RAINBOW_3)]
    internal static ClassificationTypeDefinition Rainbow3ClassificationType = null;

    [Export, Name(Constants.RAINBOW_4)]
    internal static ClassificationTypeDefinition Rainbow4ClassificationType = null;

    [Export, Name(Constants.RAINBOW_5)]
    internal static ClassificationTypeDefinition Rainbow5ClassificationType = null;

    [Export, Name(Constants.RAINBOW_6)]
    internal static ClassificationTypeDefinition Rainbow6ClassificationType = null;

    [Export, Name(Constants.RAINBOW_7)]
    internal static ClassificationTypeDefinition Rainbow7ClassificationType = null;

    [Export, Name(Constants.RAINBOW_8)]
    internal static ClassificationTypeDefinition Rainbow8ClassificationType = null;

    [Export, Name(Constants.RAINBOW_9)]
    internal static ClassificationTypeDefinition Rainbow9ClassificationType = null;

    [Export, Name(Constants.RAINBOW_ERROR)]
    internal static ClassificationTypeDefinition RainbowErrorClassificationType = null;

    [Export, Name(Constants.OBFUSCATED_TEXT)]
    internal static ClassificationTypeDefinition ObfuscatedTextType = null;

  } 
}
