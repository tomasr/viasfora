using System;

namespace Winterdom.Viasfora.Contracts {
  public interface IVsFeatures {
    bool IsSupported(String featureName);
  }
}
