using System;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora {

  public static class PkgSource {
    public static object Instance { get; private set; }
    public static Version VsVersion { get; set; }

    public static IPackageUserOptions PackageUserOptions {
      get { return Instance as IPackageUserOptions; }
    }
    public static ILogger Logger {
      get { return Instance as ILogger; }
    }
    public static IPresentationModeState PresentationMode {
      get { return Instance as IPresentationModeState; }
    }
    public static void Initialize(object obj) {
      Instance = obj;
    }
    public static void LogInfo(String format, params object[] args) {
      if ( Logger != null ) {
        Logger.LogInfo(format, args);
      }
    }
    public static void LogError(String message, Exception ex) {
      if ( Logger != null ) {
        Logger.LogError(message, ex);
      }
    }
  }
}
