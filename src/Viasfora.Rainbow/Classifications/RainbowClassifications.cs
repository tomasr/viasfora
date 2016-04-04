using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace Winterdom.Viasfora.Rainbow.Classifications {
  public static class RainbowClassifications {
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
