using System;
using System.Windows.Forms;
using Winterdom.Viasfora.Settings;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Xml;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Classifications;
using Winterdom.Viasfora.Rainbow.Classifications;
using Winterdom.Viasfora.Languages;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Options {
  public partial class MainOptionsControl : UserControl {
    private const String XML_FILTER = "XML Files (*.xml)|*.xml";
    private const String THEME_FILTER = "Viasfora Theme Files (*.vsftheme;*.json)|*.vsftheme;*.json";
    private IVsfTelemetry telemetry;

    public MainOptionsControl() {
      InitializeComponent();
      this.telemetry = SettingsContext.GetService<IVsfTelemetry>();
    }

    private void ExportSettingsButtonClick(object sender, EventArgs e) {
      this.telemetry.WriteEvent("ExportSettings");
      String filename = GetSaveAsFilename(XML_FILTER);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }

      try {
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
      } catch ( Exception ex ) {
        this.telemetry.WriteException("Failed to export settings", ex);
        MessageBox.Show(this, "Could not export settings: " + ex.Message, "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ImportSettingsButtonClick(object sender, EventArgs e) {
      this.telemetry.WriteEvent("ImportSettings");
      String filename = GetOpenFilename(XML_FILTER);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }
      try {
        var exporter = SettingsContext.GetService<ISettingsExport>();
        exporter.Load(filename);
        var store = SettingsContext.GetService<ITypedSettingsStore>();
        exporter.Import(store);
        store.Save();

        MessageBox.Show(this, "Settings imported successfully.", "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch ( Exception ex ) {
        this.telemetry.WriteException("Failed to import settings", ex);
        MessageBox.Show(this, "Could not import settings: " + ex.Message, "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ExportColors(ISettingsExport exporter) {
      ThreadHelper.ThrowIfNotOnUIThread();
      var list = GetClassifications();
      exporter.Export(list);
    }

    private ClassificationList GetClassifications() {
      ThreadHelper.ThrowIfNotOnUIThread();
      ClassificationList list = new ClassificationList(new ColorStorage(this.Site));
      list.Load(
        typeof(CodeClassificationDefinitions),
        typeof(RainbowClassifications),
        typeof(XmlClassificationDefinitions)
        );
      return list;
    }

    private void SaveCurrentThemeButtonClick(object sender, EventArgs e) {
      ThreadHelper.ThrowIfNotOnUIThread();
      this.telemetry.WriteEvent("ExportTheme");
      String filename = GetSaveAsFilename(THEME_FILTER);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }

      try {
        var classifications = GetClassifications();
        classifications.Export(filename);

        MessageBox.Show(this, "Theme saved successfully.", "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch ( Exception ex ) {
        this.telemetry.WriteException("Failed to save current theme", ex);
        MessageBox.Show(this, "Could not save theme: " + ex.Message, "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void LoadThemeButtonClick(object sender, EventArgs e) {
      ThreadHelper.ThrowIfNotOnUIThread();
      this.telemetry.WriteEvent("ImportTheme");
      String filename = GetOpenFilename(THEME_FILTER);
      if ( String.IsNullOrEmpty(filename) ) {
        return;
      }

      try {
        var classifications = GetClassifications();
        classifications.Import(filename);

        MessageBox.Show(this, "Theme imported successfully.", "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch ( Exception ex ) {
        this.telemetry.WriteException("Failed to load theme", ex);
        MessageBox.Show(this, "Could not load theme: " + ex.Message, "Viasfora", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private String GetSaveAsFilename(String filter) {
      using ( var dialog = new SaveFileDialog() ) {
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        dialog.Filter = filter;
        var result = dialog.ShowDialog(this);
        if ( result == DialogResult.OK ) {
          return dialog.FileName;
        }
        return null;
      }
    }

    private String GetOpenFilename(String filter) {
      using ( var dialog = new OpenFileDialog() ) {
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        dialog.CheckFileExists = true;
        dialog.Filter = filter;
        var result = dialog.ShowDialog(this);
        if ( result == DialogResult.OK ) {
          return dialog.FileName;
        }
        return null;
      }
    }
  }
}
