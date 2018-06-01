using System;
using Winterdom.Viasfora.Compatibility;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora {
  public static class PkgSource {

    private static Lazy<ILogger> logger = new Lazy<ILogger>(GetLogger);

    public static void LogError(String message, Exception ex) {
      if ( logger.Value != null ) {
        logger.Value.LogError(message, ex);
      }
    }

    private static ILogger GetLogger() {
      var model = new SComponentModel();
      return model.GetService<ILogger>();
    }
  }
}
