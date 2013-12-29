using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winterdom.Viasfora.Options {
  public partial class PresentationModeDialogPage : UserControl {

    public int DefaultZoom {
      get { return defaultZoom.Value; }
      set { defaultZoom.Value = value; }
    }
    public int EnabledZoom {
      get { return enabledZoom.Value; }
      set { enabledZoom.Value = value; }
    }

    public PresentationModeDialogPage() {
      InitializeComponent();
    }

  }
}
