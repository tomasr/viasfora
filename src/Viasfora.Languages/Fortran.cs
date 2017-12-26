using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class Fortran : LanguageInfo, ILanguage {
    public const String ContentType = "Fortran";
    protected override String[] SupportedContentTypes
      => new String[] { ContentType };
    public ILanguageSettings Settings { get; private set; }

    [ImportingConstructor]
    public Fortran(ITypedSettingsStore store) {
      this.Settings = new FortranSettings(store);
    }

    protected override IBraceScanner NewBraceScanner()
      => new DefaultBraceScanner();
  }

  class FortranSettings : LanguageSettings {
    protected override String[] ControlFlowDefaults => new String[] {
       "if", "then", "end if", "endif", "else", "call", "return",
       "do", "end do", "enddo", "while", "select", "end select"
      };
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => new String[] {
       "public", "private"
      };

    public FortranSettings(ITypedSettingsStore store)
      : base (Constants.Fortran, store) {
    }
  }
}
