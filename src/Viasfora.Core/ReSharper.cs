using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Winterdom.Viasfora {
  public static class ReSharper {
    public const String PackageId = "0C6E6407-13FC-4878-869A-C8B4016C57FE";
    private static Lazy<bool> isInstalled = new Lazy<bool>(GetInstalled);

    public static bool Installed => isInstalled.Value;

    private static bool GetInstalled() {
      ThreadHelper.ThrowIfNotOnUIThread();
      IVsShell vsShell = (IVsShell)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsShell));
      if ( vsShell != null ) {
        Guid pkgId = new Guid(PackageId);
        int installed = 0;
        int hr = vsShell.IsPackageInstalled(ref pkgId, out installed);
        if ( ErrorHandler.Succeeded(hr) ) {
          return installed != 0;
        }
      }
      // can't tell if it is installed or not
      return false;
    }
  }
}
