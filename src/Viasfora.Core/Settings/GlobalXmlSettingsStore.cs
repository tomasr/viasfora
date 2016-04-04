using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Winterdom.Viasfora.Settings {
  [Export(typeof(ISettingsStore))]
  public class GlobalXmlSettingsStore : ISettingsStore {
    const String FILE_NAME = "viasfora.xml";
    private String filePath;
    Dictionary<String, String> settings = new Dictionary<String, String>();

    public GlobalXmlSettingsStore() : this(null) {
    }
    public GlobalXmlSettingsStore(String file) {
      ConfigurePath(file);
      Load();
    }

    public String Get(String name) {
      String val;
      if ( settings.TryGetValue(name, out val) ) {
        return val;
      }
      return null;
    }

    public void Set(String name, String value) {
      settings[name] = value;
    }

    public void Load() {
      if ( File.Exists(filePath) ) {
        try {
          XDocument doc = XDocument.Load(filePath);
          foreach ( var element in doc.Root.Elements() ) {
            settings[element.Name.LocalName] = element.Value;
          }
        } catch ( XmlException ex ) {
          PkgSource.LogInfo("Error loading '{0}': {1}", filePath, ex);
        }
      }
    }

    public void Save() {
      using ( var xw = XmlWriter.Create(filePath) ) {
        xw.WriteStartElement("viasfora");
        foreach ( String key in settings.Keys ) {
          String value = settings[key];
          if ( value != null ) {
            xw.WriteElementString(key, settings[key]);
          }
        }
        xw.WriteEndElement();
      }
    }

    private void ConfigurePath(string filePath) {
      String folder = null;
      String fileName = FILE_NAME;
      if ( String.IsNullOrEmpty(filePath) ) {
        folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Viasfora"
          );
      } else {
        folder = Path.GetDirectoryName(filePath);
        fileName = Path.GetFileName(filePath);
      }
      if ( !Directory.Exists(folder) ) {
        Directory.CreateDirectory(folder);
      }
      this.filePath = Path.Combine(folder, fileName);
    }
  }
}
