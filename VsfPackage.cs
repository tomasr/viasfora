using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace Winterdom.Viasfora {
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [InstalledProductRegistration("#110", "#111", "1.0", IconResourceID = 400)]
  [Guid(Constants.VSPackage)]
  [ProvideOptionPage(typeof(Options.GeneralOptionsPage), "Viasfora", "General", 101, 101, true)]
  public sealed class VsfPackage : Package {

    public static VsfPackage Instance { get; private set; }

    protected override void Initialize() {
      base.Initialize();
      Instance = this;
      Trace.WriteLine("Initializing VsfPackage");

      OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if ( null != mcs ) {
      }
    }
  }
}
