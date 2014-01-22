using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Winterdom.Viasfora.Languages;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio;
using System.IO;
using System.Reflection;
using Winterdom.Viasfora.Commands;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;

namespace Winterdom.Viasfora {
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
  [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
  [InstalledProductRegistration("#110", "#111", "1.0", IconResourceID = 400)]
  [Guid(Guids.VSPackage)]
  [ProvideOptionPage(typeof(Options.GeneralOptionsPage), "Viasfora", "General", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.PresentationModeOptionsPage), "Viasfora", "Presentation Mode", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.AllLanguagesOptionsPage), "Viasfora", "Languages", 0, 0, false)]
  [ProvideOptionPage(typeof(Options.CSharpOptionsPage), "Viasfora\\Languages", "C#", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.CppOptionsPage), "Viasfora\\Languages", "C/C++", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.JScriptOptionsPage), "Viasfora\\Languages", "JavaScript", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.VBOptionsPage), "Viasfora\\Languages", "Basic", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.FSharpOptionsPage), "Viasfora\\Languages", "F#", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.SqlOptionsPage), "Viasfora\\Languages", "SQL", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.TypeScriptOptionsPage), "Viasfora\\Languages", "TypeScript", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.PythonOptionsPage), "Viasfora\\Languages", "Python", 0, 0, true)]
  [ProvideMenuResource(1000, 1)]
  public sealed class VsfPackage : Package {

    private static readonly LanguageInfo DefaultLanguage = new DefaultLanguage();
    private static List<LanguageInfo> languageList;
    public static VsfPackage Instance { get; private set; }

    public static bool PresentationModeTurnedOn { get; set; }
    public static EventHandler PresentationModeChanged { get; set; }

    private Version vsVersion;
    private IVsActivityLog activityLog;
    private List<VsCommand> commands = new List<VsCommand>();

    static VsfPackage() {
      languageList = new List<LanguageInfo>();
      languageList.Add(new Cpp());
      languageList.Add(new CSharp());
      languageList.Add(new JScript());
      languageList.Add(new VB());
      languageList.Add(new FSharp());
      languageList.Add(new Sql());
      languageList.Add(new TypeScript());
      languageList.Add(new Python());
    }

    public static LanguageInfo LookupLanguage(IContentType contentType) {
      foreach ( LanguageInfo li in languageList ) {
        if ( li.MatchesContentType(contentType) )
          return li;
      }
      return new DefaultLanguage();
    }

    public static int GetPresentationModeZoomLevel() {
      return PresentationModeTurnedOn
        ? VsfSettings.PresentationModeEnabledZoomLevel
        : VsfSettings.PresentationModeDefaultZoomLevel;
    }

    protected override void Initialize() {
      base.Initialize();
      Instance = this;
      InitializeActivityLog();
      LogInfo("Initializing VsfPackage");
      vsVersion = FindVSVersion();

      OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if ( null != mcs ) {
        InitializeViewMenuCommands(mcs);
        InitializeTextEditorCommands(mcs);
      }
    }

    private void InitializeActivityLog() {
      this.activityLog = (IVsActivityLog)GetService(typeof(SVsActivityLog));
    }

    public static void LogInfo(String format, params object[] args) {
      var log = Instance.activityLog;
      if ( log != null ) {
        log.LogEntry(
          (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
          "Viasfora",
          String.Format(format, args)
        );
      }
    }


    // see http://msdn.microsoft.com/en-us/library/vstudio/microsoft.visualstudio.platformui.environmentcolors.aspx
    // for available styles.
    public object FindColorResource(String name) {
      if ( vsVersion.Major <= 10 ) {
        return null;
      }
      Assembly assembly = Assembly.Load(String.Format(
        "Microsoft.VisualStudio.Shell.{0}.0, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        vsVersion.Major
        ));
      if ( assembly != null ) {
        Type type = assembly.GetType("Microsoft.VisualStudio.PlatformUI.EnvironmentColors");
        var prop = type.GetProperty(name);
        return prop.GetValue(null, null);
      }
      return null;
    }

    private void InitializeViewMenuCommands(OleMenuCommandService mcs) {
      commands.Add(new PresentationModeCommand(this, mcs));
    }
    private void InitializeTextEditorCommands(OleMenuCommandService mcs) {
      commands.Add(new AddOutliningCommand(this, mcs));
      commands.Add(new RemoveOutliningCommand(this, mcs));
      commands.Add(new ClearOutliningCommand(this, mcs));
    }


    private static Version FindVSVersion() {
      String key = Instance.UserRegistryRoot.Name;
      String last = Path.GetFileName(key);
      last = last.Substring(0, last.IndexOf('.'));
      int version;
      if ( Int32.TryParse(last, out version) ) {
        return new Version(version, 0, 0, 0);
      }
      return new Version(10, 0, 0, 0);
    }
  }
}
