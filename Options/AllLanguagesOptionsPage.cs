using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.AllLanguagesOptions)]
  public class AllLanguagesOptionsPage : DialogPage {
    protected override IWin32Window Window {
      get {
        return new UserControl();
      }
    }
  }
}
