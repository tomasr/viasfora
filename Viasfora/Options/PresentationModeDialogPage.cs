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

    public bool PMEnabled {
      get { return enableCheckbox.Checked; }
      set { enableCheckbox.Checked = value; }
    }
    public int DefaultZoom {
      get { return defaultZoom.Value; }
      set { defaultZoom.Value = value; }
    }
    public int EnabledZoom {
      get { return enabledZoom.Value; }
      set { enabledZoom.Value = value; }
    }
    public bool IncludeEnvironmentFonts {
      get { return includeEnvFontsCheckBox.Checked; }
      set { includeEnvFontsCheckBox.Checked = value; }
    }

    public PresentationModeDialogPage() {
      InitializeComponent();
    }

    private void OnEnableCheckboxChecked(object sender, EventArgs e) {
      enabledZoom.Enabled = enableCheckbox.Checked;
      defaultZoom.Enabled = enableCheckbox.Checked;
    }

  }
}
