using System;
using System.Windows.Forms;

namespace Winterdom.Viasfora.Options {
  public partial class PresentationModeDialogPage : UserControl {

    public bool PMEnabled {
      get { return this.enableCheckbox.Checked; }
      set { this.enableCheckbox.Checked = value; }
    }
    public int DefaultZoom {
      get { return this.defaultZoom.Value; }
      set { this.defaultZoom.Value = value; }
    }
    public int EnabledZoom {
      get { return this.enabledZoom.Value; }
      set { this.enabledZoom.Value = value; }
    }
    public bool IncludeEnvironmentFonts {
      get { return this.includeEnvFontsCheckBox.Checked; }
      set { this.includeEnvFontsCheckBox.Checked = value; }
    }

    public PresentationModeDialogPage() {
      InitializeComponent();
    }

    private void OnEnableCheckboxChecked(object sender, EventArgs e) {
      this.enabledZoom.Enabled = this.enableCheckbox.Checked;
      this.defaultZoom.Enabled = this.enableCheckbox.Checked;
    }

  }
}
