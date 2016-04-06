using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Winterdom.Viasfora.Commands;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Text;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora {
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
  [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
  [Guid(Guids.VSPackage)]
  [ProvideOptionPage(typeof(Options.GeneralOptionsPage), "Viasfora", "General", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.RainbowOptionsPage), "Viasfora", "Rainbow Braces", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.XmlOptionsPage), "Viasfora", "XML Editor", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.PresentationModeOptionsPage), "Viasfora", "Presentation Mode", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.IntellisenseOptions), "Viasfora", "Intellisense", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.TextObfuscationOptionsPage), "Viasfora", "Text Hiding", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.AllLanguagesOptionsPage), "Viasfora", "Languages", 0, 0, false)]
  [ProvideOptionPage(typeof(Options.CSharpOptionsPage), "Viasfora\\Languages", "C#", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.CppOptionsPage), "Viasfora\\Languages", "C/C++", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.JScriptOptionsPage), "Viasfora\\Languages", "JavaScript", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.VBOptionsPage), "Viasfora\\Languages", "Basic", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.FSharpOptionsPage), "Viasfora\\Languages", "F#", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.SqlOptionsPage), "Viasfora\\Languages", "SQL", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.TypeScriptOptionsPage), "Viasfora\\Languages", "TypeScript", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.PythonOptionsPage), "Viasfora\\Languages", "Python", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.ROptionsPage), "Viasfora\\Languages", "R", 0, 0, true)]
  [ProvideOptionPage(typeof(Options.PowerShellOptionsPage), "Viasfora\\Languages", "PowerShell", 0, 0, true)]
  [ProvideMenuResource(1000, 1)]
  public sealed class VsfPackage
    : Package,
      IPackageUserOptions,
      IPresentationModeState,
      ILogger
    {
    public const String USER_OPTIONS_KEY = "VsfUserOptions";

    public PresentationModeFontChanger FontChanger { get; private set; } 
    private IVsActivityLog activityLog;
    private List<VsCommand> commands = new List<VsCommand>();
    private byte[] userOptions;

    protected override void Initialize() {
      base.Initialize();
      LogInfo("Initializing VsfPackage");
      PkgSource.Initialize(this);
      PkgSource.VsVersion = FindVSVersion();
      InitializeTelemetry();
      InitializeActivityLog();

      this.FontChanger = new PresentationModeFontChanger(this);

      OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if ( null != mcs ) {
        InitializeViewMenuCommands(mcs);
        InitializeTextEditorCommands(mcs);
      }

      this.AddOptionKey(USER_OPTIONS_KEY);
    }

    protected override void Dispose(bool disposing) {

      if ( disposing && PresentationModeTurnedOn ) {
        FontChanger.TurnOff(notifyChanges: false);
      }

      base.Dispose(disposing);
    }

    protected override void OnLoadOptions(string key, Stream stream) {
      base.OnLoadOptions(key, stream);
      if ( key == USER_OPTIONS_KEY ) {
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        this.userOptions = data;
      }
    }

    protected override void OnSaveOptions(string key, Stream stream) {
      base.OnSaveOptions(key, stream);
      if ( key == USER_OPTIONS_KEY && userOptions != null ) {
        stream.Write(userOptions, 0, userOptions.Length);
      }
    }

    private void InitializeActivityLog() {
      this.activityLog = (IVsActivityLog)GetService(typeof(SVsActivityLog));
    }

    private void InitializeTelemetry() {
      var dte = (EnvDTE80.DTE2)GetService(typeof(SDTE));
      Telemetry.Initialize(dte);
    }

    public void LogInfo(String format, params object[] args) {
      var log = this.activityLog;
      if ( log != null ) {
        log.LogEntry(
          (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
          "Viasfora",
          String.Format(format, args)
        );
      }
    }
    public void LogError(String message, Exception ex) {
      var log = this.activityLog;
      if ( log != null ) {
        log.LogEntry(
          (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
          "Viasfora",
          String.Format("{0}. Exception: {1}", message, ex)
        );
      }
      Telemetry.WriteException(message, ex);
    }

    private void InitializeViewMenuCommands(OleMenuCommandService mcs) {
      commands.Add(new PresentationModeCommand(this, mcs));
      commands.Add(new ObfuscateTextCommand(this, mcs));
    }
    private void InitializeTextEditorCommands(OleMenuCommandService mcs) {
      commands.Add(new AddOutliningCommand(this, mcs));
      commands.Add(new RemoveOutliningCommand(this, mcs));
      commands.Add(new ClearOutliningCommand(this, mcs));
      commands.Add(new SelectionOutliningCommand(this, mcs));
      commands.Add(new CompleteWordCommand(this, mcs));
    }

    private Version FindVSVersion() {
      var dte = (EnvDTE80.DTE2)GetService(typeof(SDTE));
      return Version.Parse(dte.Version);
    }

    internal static ISettingsStore GetGlobalSettingsStore() {
      return new GlobalXmlSettingsStore(null);
    }

    void IPackageUserOptions.Write(byte[] options) {
      this.userOptions = options;
    }
    byte[] IPackageUserOptions.Read() {
      return this.userOptions;
    }

    //
    // Presentation mode Support
    //
    public event EventHandler PresentationModeChanged;
    public bool PresentationModeTurnedOn { get; private set; }
    public int GetPresentationModeZoomLevel() {
      var settings = SettingsContext.GetSettings();
      return PresentationModeTurnedOn
        ? settings.PresentationModeEnabledZoom
        : settings.PresentationModeDefaultZoom;
    }
    public void TogglePresentationMode() {
      PresentationModeTurnedOn = !PresentationModeTurnedOn;
      if ( PresentationModeChanged != null ) {
        PresentationModeChanged(this, EventArgs.Empty);
      }
      if ( PresentationModeTurnedOn ) {
        FontChanger.TurnOn();
        Telemetry.WriteEvent("Presentation Mode");
      } else {
        FontChanger.TurnOff();
      }
    }
    
    public T GetService<T>() {
      return (T)GetService(typeof(T));
    }

  }
}
