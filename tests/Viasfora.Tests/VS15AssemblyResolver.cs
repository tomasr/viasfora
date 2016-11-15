using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Viasfora.Tests {
  public class VS15AssemblyResolverFixture {
    const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
    static readonly String vsInstallDir;
    static readonly String[] assemblyLocations;
    static VS15AssemblyResolverFixture() {
      vsInstallDir = TryFindVS15InstallDir();
      assemblyLocations = new String[] {
                Path.Combine(vsInstallDir, @"Common7\IDE"),
                Path.Combine(vsInstallDir, @"Common7\IDE\PrivateAssemblies"),
                Path.Combine(vsInstallDir, @"Common7\IDE\PublicAsemblies"),
                Path.Combine(vsInstallDir, @"Common7\IDE\CommonExtensions\Microsoft\Editor"),
            };
      AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
    }

    private static String TryFindVS15InstallDir() {
      var setupConfig = GetVsConfig();
      if ( setupConfig != null ) {
        var instances = setupConfig.EnumInstances();
        ISetupInstance[] idata = new ISetupInstance[1];
        int numFetch = 0;
        instances.Next(1, idata, out numFetch);
        if ( numFetch > 0 ) {
          return idata[0].GetInstallationPath();
        }
      }
      throw new InvalidOperationException("Visual Studio installation directory not found.");
    }

    private static ISetupConfiguration GetVsConfig() {
      try {
        // Try to CoCreate the class object.
        return new SetupConfiguration();
      } catch (COMException ex) when (ex.HResult == REGDB_E_CLASSNOTREG) {
        // Try to get the class object using app-local call.
        ISetupConfiguration setupConfig;
        var result = GetSetupConfiguration(out setupConfig, IntPtr.Zero);
        return result < 0 ? null : setupConfig;
      }
    }

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
      var name = new AssemblyName(args.Name);
      foreach (String dir in assemblyLocations) {
        String path = Path.Combine(dir, name.Name + ".dll");
        if (File.Exists(path)) {
          return Assembly.LoadFrom(path);
        }
      }
      return null;
    }

    [DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
    private static extern int GetSetupConfiguration(
    [MarshalAs(UnmanagedType.Interface), Out] out ISetupConfiguration configuration, IntPtr reserved);
  }

  [CollectionDefinition("DependsOnVS")]
  public class DependsOnVSCollection : ICollectionFixture<VS15AssemblyResolverFixture> {
  }
}
