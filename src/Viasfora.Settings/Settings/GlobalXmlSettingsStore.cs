using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Winterdom.Viasfora.Settings {
  [PartCreationPolicy(CreationPolicy.Shared)]
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
      if ( this.settings.TryGetValue(name, out string val) ) {
        return val;
      }
      return null;
    }

    public void Set(String name, String value) {
      this.settings[name] = value;
    }

    public void Load() {
      if ( File.Exists(this.filePath) ) {
        XDocument doc = XDocument.Load(this.filePath);
        foreach ( var element in doc.Root.Elements() ) {
          this.settings[element.Name.LocalName] = element.Value;
        }
      }
    }

    public void Save() {
      using ( var xw = XmlWriter.Create(this.filePath) ) {
        xw.WriteStartElement("viasfora");
        foreach ( String key in this.settings.Keys ) {
          String value = this.settings[key];
          if ( value != null ) {
            xw.WriteElementString(key, this.settings[key]);
          }
        }
        xw.WriteEndElement();
      }
    }

    private void ConfigurePath(string filePath) {
      String folder = null;
      if ( String.IsNullOrEmpty(filePath) ) {
        filePath = GetDefaultFilePath();
      }

      folder = Path.GetDirectoryName(filePath);
      if ( !Directory.Exists(folder) ) {
        Directory.CreateDirectory(folder);
      }
      this.filePath = filePath;
    }

    private static String GetDefaultFilePath() {
      var envValue = Environment.GetEnvironmentVariable("VIASFORA_SETTINGS");
      if ( !String.IsNullOrEmpty(envValue) ) {
        return envValue;
      } else {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Viasfora", FILE_NAME
          );
      }
    }
  }
}
