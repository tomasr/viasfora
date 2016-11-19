using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using Winterdom.Viasfora.Compatibility;
using Winterdom.Viasfora.Contracts;
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
    public static String MakeRelativePath(String toPath) {
      String solutionFile = GetSolutionPath();
      return MakeRelativePath(solutionFile, toPath);
    }
    public static String MakeRelativePath(String fromPath, String toPath) {
      if ( String.IsNullOrEmpty(fromPath) ) {
        return toPath;
      }
      // based on: http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
      Uri fromUri, toUri;
      if ( Uri.TryCreate(fromPath, UriKind.Absolute, out fromUri)
        && Uri.TryCreate(toPath, UriKind.Absolute, out toUri) ) {
        if ( fromUri.Scheme != toUri.Scheme ) { return toPath; } // path can't be made relative.

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if ( toUri.Scheme.ToUpperInvariant() == "FILE" ) {
          relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        return relativePath;
      }
      // avoid invalid URI format exceptions
      return toPath;
    }

    public static ISolutionUserSettings GetUserSettings() {
      /*
      String solutionPath = GetSolutionPath();
      if ( String.IsNullOrEmpty(solutionPath) ) {
        return null;
      }
      IPersistSettings persist = new FilePersistUserSettings(solutionPath);
      */
      IPersistSettings persist = new SuoPersistUserSettings(PkgSource.PackageUserOptions);
      return new SolutionUserSettings(persist);
    }

    private static void CheckError(int hr, String operation) {
      if ( hr != Constants.S_OK ) {
        var ex = new InvalidOperationException(String.Format("{0} returned 0x{1:x8}", operation, hr));
        PkgSource.LogError(operation, ex);
        throw ex;
      }
    }
  }
}
