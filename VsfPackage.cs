using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Winterdom.Viasfora.Text;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora {
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [InstalledProductRegistration("#110", "#111", "1.0", IconResourceID = 400)]
  [Guid(Guids.VSPackage)]
  [ProvideOptionPage(typeof(Options.GeneralOptionsPage), "Viasfora", "General", 101, 102, true)]
  [ProvideOptionPage(typeof(Options.CSharpOptionsPage), "Viasfora", "C#", 101, 103, true)]
  [ProvideOptionPage(typeof(Options.CppOptionsPage), "Viasfora", "C/C++", 101, 104, true)]
  [ProvideOptionPage(typeof(Options.JScriptOptionsPage), "Viasfora", "JavaScript", 101, 105, true)]
  [ProvideOptionPage(typeof(Options.VBOptionsPage), "Viasfora", "Basic", 101, 106, true)]
  [ProvideOptionPage(typeof(Options.FSharpOptionsPage), "Viasfora", "F#", 101, 107, true)]
  [ProvideOptionPage(typeof(Options.SqlOptionsPage), "Viasfora", "SQL", 101, 108, true)]
  public sealed class VsfPackage : Package {

    private static List<LanguageInfo> languageList;
    public static VsfPackage Instance { get; private set; }

    static VsfPackage() {
      languageList = new List<LanguageInfo>();
      languageList.Add(new Cpp());
      languageList.Add(new CSharp());
      languageList.Add(new JScript());
      languageList.Add(new VB());
      languageList.Add(new FSharp());
      languageList.Add(new Sql());
    }

    public static LanguageInfo LookupLanguage(IContentType contentType) {
      foreach ( LanguageInfo li in languageList ) {
        if ( li.MatchesContentType(contentType) )
          return li;
      }
      return null;
    }
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
