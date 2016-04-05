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
    private UserControl dialog = new UserControl();
    protected override IWin32Window Window {
      get { return dialog; }
    }
  }
}
