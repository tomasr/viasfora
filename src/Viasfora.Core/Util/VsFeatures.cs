using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Util {
  [Export(typeof(IVsFeatures))]
  public class VsFeatures : IVsFeatures {
    public bool IsSupported(string featureName) {
      switch ( featureName ) {
        case KnownFeatures.TooltipApi:
          return IsQuickInfoSourceDeprecated();
      }
      throw new InvalidOperationException("Unknown feature: " + featureName);
    }

    private bool IsQuickInfoSourceDeprecated() {
      return typeof(IQuickInfoSource)
            .GetCustomAttributes(typeof(ObsoleteAttribute), false)
            .Length > 0;
    }
  }
}
