using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Viasfora.Tests {
  public class VSAssemblyResolverFixture {
    const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
    static readonly String vsInstallDir;
    static readonly String[] assemblyLocations;

    public const String VSVERSION = "17";
    public const String VSASMVERSION = "17.0.0.0";
    static VSAssemblyResolverFixture() {
      vsInstallDir = TryFindVSInstallDir();
      if ( !String.IsNullOrEmpty(vsInstallDir) ) {
        assemblyLocations = new String[] {
                Path.Combine(vsInstallDir, @"Common7\IDE\CommonExtensions\Microsoft\Editor"),
                Path.Combine(vsInstallDir, @"Common7\IDE"),
                Path.Combine(vsInstallDir, @"Common7\IDE\PrivateAssemblies"),
                Path.Combine(vsInstallDir, @"Common7\IDE\PublicAssemblies"),
            };
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
      }
    }

    private static String TryFindVSInstallDir() {
      var setupConfig = GetVsConfig();
      if ( setupConfig != null ) {
        var instances = setupConfig.EnumInstances();
        ISetupInstance[] idata = new ISetupInstance[10];
        int numFetch = 0;
        instances.Next(10, idata, out numFetch);
        for ( int i = 0; i < numFetch; i++ ) {
          String version = idata[i].GetInstallationVersion();
          if ( version.StartsWith(VSVERSION) ) {
            return idata[i].GetInstallationPath();
          }
        }
      }
      return null;
    }

    private static ISetupConfiguration GetVsConfig() {
      try {
        // Try to CoCreate the class object.
        return new SetupConfiguration();
      } catch ( COMException ex ) when ( ex.HResult == REGDB_E_CLASSNOTREG ) {
        // Try to get the class object using app-local call.
        try {
          ISetupConfiguration setupConfig;
          var result = GetSetupConfiguration(out setupConfig, IntPtr.Zero);
          return result < 0 ? null : setupConfig;
        } catch ( DllNotFoundException ) {
          return null;
        }
      }
    }

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
      var name = new AssemblyName(args.Name);
      foreach ( String dir in assemblyLocations ) {
        String path = Path.Combine(dir, name.Name + ".dll");
        if ( File.Exists(path) ) {
          return Assembly.LoadFrom(path);
        }
      }
      Console.WriteLine("Could not resolve: {0}", name);
      return null;
    }

    [DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
    private static extern int GetSetupConfiguration(
    [MarshalAs(UnmanagedType.Interface), Out] out ISetupConfiguration configuration, IntPtr reserved);
  }

  [CollectionDefinition("DependsOnVS")]
  public class DependsOnVSCollection : ICollectionFixture<VSAssemblyResolverFixture> {
  }
}
