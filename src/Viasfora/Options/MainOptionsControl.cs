using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Xml;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Options {
  public partial class MainOptionsControl : UserControl {
    public MainOptionsControl() {
      InitializeComponent();
    }

    private void ExportButtonClick(object sender, EventArgs e) {
      String filename = GetFileName();
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }

      var exporter = SettingsContext.GetService<ISettingsExport>();
      exporter.Export(SettingsContext.GetSettings());
      exporter.Export(SettingsContext.GetService<IRainbowSettings>());
      exporter.Export(SettingsContext.GetService<IXmlSettings>());

      var languageFactory = SettingsContext.GetService<ILanguageFactory>();
      foreach ( var lang in languageFactory.GetAllLanguages() ) {
        exporter.Export(lang.Settings);
      }
      exporter.Save(filename);
      MessageBox.Show(this, "Settings exported successfully.", "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ImportButtonClick(object sender, EventArgs e) {

    }

    private String GetFileName() {
      using ( var dialog = new SaveFileDialog() ) {
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        dialog.Filter = "XML Files (*.xml)|*.xml";
        var result = dialog.ShowDialog(this);
        if ( result == DialogResult.OK ) {
          return dialog.FileName;
        }
        return null;
      }
    }
  }
}
