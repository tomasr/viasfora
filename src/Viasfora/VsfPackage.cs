using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Winterdom.Viasfora.Commands;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Compatibility;

namespace Winterdom.Viasfora {
  [PackageRegistration(UseManagedResourcesOnly = true)]
  //[ProvideAutoLoad(VSConstants.VsEditorFactoryGuid.TextEditor_string)]
  [Guid(Guids.VSPackage)]
  [InstalledProductRegistration("#110", "#111", productId: VsfVersion.Version, IconResourceID = 400)]
  [ProvideOptionPage(typeof(Options.GeneralOptionsPage), "Viasfora", "General", 200, 310, true)]
  [ProvideOptionPage(typeof(Options.MainOptionsPage), "Viasfora", "Main", 200, 315, true)]
  [ProvideOptionPage(typeof(Options.RainbowOptionsPage), "Viasfora", "Rainbow Braces", 200, 320, true)]
  [ProvideOptionPage(typeof(Options.XmlOptionsPage), "Viasfora", "XML Editor", 200, 330, true)]
  [ProvideOptionPage(typeof(Options.PresentationModeOptionsPage), "Viasfora", "Presentation Mode", 200, 340, true)]
  [ProvideOptionPage(typeof(Options.TextObfuscationOptionsPage), "Viasfora", "Text Hiding", 200, 360, true)]
  [ProvideOptionPage(typeof(Options.AllLanguagesOptionsPage), "Viasfora", "Languages", 200, 370, false)]
  [ProvideOptionPage(typeof(Options.CSharpOptionsPage), "Viasfora\\Languages", "C#", 210, 371, true)]
  [ProvideOptionPage(typeof(Options.CppOptionsPage), "Viasfora\\Languages", "C/C++", 210, 372, true)]
  [ProvideOptionPage(typeof(Options.JScriptOptionsPage), "Viasfora\\Languages", "JavaScript", 210, 373, true)]
  [ProvideOptionPage(typeof(Options.VBOptionsPage), "Viasfora\\Languages", "Basic", 210, 374, true)]
  [ProvideOptionPage(typeof(Options.FSharpOptionsPage), "Viasfora\\Languages", "F#", 210, 375, true)]
  [ProvideOptionPage(typeof(Options.SqlOptionsPage), "Viasfora\\Languages", "SQL", 210, 376, true)]
  [ProvideOptionPage(typeof(Options.USqlOptionsPage), "Viasfora\\Languages", "U-SQL", 210, 377, true)]
  [ProvideOptionPage(typeof(Options.TypeScriptOptionsPage), "Viasfora\\Languages", "TypeScript", 210, 378, true)]
  [ProvideOptionPage(typeof(Options.PythonOptionsPage), "Viasfora\\Languages", "Python", 210, 379, true)]
  [ProvideOptionPage(typeof(Options.ROptionsPage), "Viasfora\\Languages", "R", 210, 380, true)]
  [ProvideOptionPage(typeof(Options.PowerShellOptionsPage), "Viasfora\\Languages", "PowerShell", 210, 381, true)]
  [ProvideOptionPage(typeof(Options.FortranOptionsPage), "Viasfora\\Languages", "Fortran", 210, 382, true)]
  [ProvideMenuResource(1000, 1)]
  public sealed class VsfPackage : Package, IPackageUserOptions {
    public const String USER_OPTIONS_KEY = "VsfUserOptions";

    public PresentationModeFontChanger FontChanger { get; private set; } 
    private List<VsCommand> commands = new List<VsCommand>();
    private byte[] userOptions;

    protected override void Initialize() {
      base.Initialize();

      if ( GetService(typeof(IMenuCommandService)) is OleMenuCommandService mcs ) {
        InitializeViewMenuCommands(mcs);
        InitializeTextEditorCommands(mcs);
      }

      this.AddOptionKey(USER_OPTIONS_KEY);
    }

    protected override void Dispose(bool disposing) {
      var model = new SComponentModel();
      var ps = model.GetService<IPresentationModeState>();
      if ( disposing && ps.PresentationModeTurnedOn ) {
        ps.TurnOff(notifyChanges: false);
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
      if ( key == USER_OPTIONS_KEY && this.userOptions != null ) {
        stream.Write(this.userOptions, 0, this.userOptions.Length);
      }
    }

    private void InitializeViewMenuCommands(OleMenuCommandService mcs) {
      this.commands.Add(new PresentationModeCommand(this, mcs));
      this.commands.Add(new ObfuscateTextCommand(this, mcs));
    }
    private void InitializeTextEditorCommands(OleMenuCommandService mcs) {
      this.commands.Add(new AddOutliningCommand(this, mcs));
      this.commands.Add(new RemoveOutliningCommand(this, mcs));
      this.commands.Add(new ClearOutliningCommand(this, mcs));
      this.commands.Add(new SelectionOutliningCommand(this, mcs));
      this.commands.Add(new CompleteWordCommand(this, mcs));
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

    public T GetService<T>() {
      return (T)GetService(typeof(T));
    }
  }
}
