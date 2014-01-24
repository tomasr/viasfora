using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora {
  public static class VsSolution {
    public static String GetSolutionPath() {
      IVsSolution solution = (IVsSolution)
        ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
      if ( solution == null ) {
        return null;
      }
      String solutionDir, solutionFile, userOptsFile;
      int hr = solution.GetSolutionInfo(out solutionDir, out solutionFile, out userOptsFile);
      CheckError(hr, "GetSolutionInfo");
      return String.IsNullOrEmpty(solutionDir) ? null : Path.GetFullPath(solutionDir);
    }

    public static ISolutionUserSettings GetUserSettings() {
      String solutionPath = GetSolutionPath();
      if ( String.IsNullOrEmpty(solutionPath) ) {
        return null;
      }
      return new SolutionUserSettings(solutionPath);
    }

    private static void CheckError(int hr, String operation) {
      if ( hr != Constants.S_OK ) {
        VsfPackage.LogInfo("{0} returned 0x{1:x8}", operation, hr);
        throw new InvalidOperationException(String.Format("{0} returned 0x{1:x8}", operation, hr));
      }
    }
  }
}
