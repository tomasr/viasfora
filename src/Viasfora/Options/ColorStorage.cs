using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Winterdom.Viasfora.Options {
  public class ColorStorage {
    public IVsFontAndColorStorage Storage { get; set; }
    public IVsFontAndColorUtilities Utilities { get; set; } 
    public IVsUIShell2 Shell { get; set; }
  }
}
