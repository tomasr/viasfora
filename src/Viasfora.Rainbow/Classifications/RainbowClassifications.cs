using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace Winterdom.Viasfora.Rainbow.Classifications {
  public static class RainbowClassifications {
    [Export, Name(Rainbows.Rainbow1)]
    internal static ClassificationTypeDefinition Rainbow1ClassificationType = null;

    [Export, Name(Rainbows.Rainbow2)]
    internal static ClassificationTypeDefinition Rainbow2ClassificationType = null;

    [Export, Name(Rainbows.Rainbow3)]
    internal static ClassificationTypeDefinition Rainbow3ClassificationType = null;

    [Export, Name(Rainbows.Rainbow4)]
    internal static ClassificationTypeDefinition Rainbow4ClassificationType = null;

    [Export, Name(Rainbows.Rainbow5)]
    internal static ClassificationTypeDefinition Rainbow5ClassificationType = null;

    [Export, Name(Rainbows.Rainbow6)]
    internal static ClassificationTypeDefinition Rainbow6ClassificationType = null;

    [Export, Name(Rainbows.Rainbow7)]
    internal static ClassificationTypeDefinition Rainbow7ClassificationType = null;

    [Export, Name(Rainbows.Rainbow8)]
    internal static ClassificationTypeDefinition Rainbow8ClassificationType = null;

    [Export, Name(Rainbows.Rainbow9)]
    internal static ClassificationTypeDefinition Rainbow9ClassificationType = null;

    [Export, Name(Rainbows.RainbowError)]
    internal static ClassificationTypeDefinition RainbowErrorClassificationType = null;

  }
}
