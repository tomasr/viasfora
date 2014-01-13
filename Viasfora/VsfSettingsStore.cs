using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Winterdom.Viasfora {
  public class VsfSettingsStore {
    const String FILE_NAME = "viasfora.xml";
    Dictionary<String, String> settings = new Dictionary<String, String>();

    public VsfSettingsStore() {
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
      String file = GetUserSettingsPath();
      if ( File.Exists(file) ) {
        XDocument doc = XDocument.Load(file);
        foreach ( var element in doc.Root.Elements() ) {
          settings[element.Name.LocalName] = element.Value;
        }
      }
    }

    public void Write() {
      String file = GetUserSettingsPath();
      using ( var xw = XmlWriter.Create(file) ) {
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

    private String GetUserSettingsPath() {
      String folder = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
          "Viasfora"
        );
      if ( !Directory.Exists(folder) ) {
        Directory.CreateDirectory(folder);
      }
      return Path.Combine(folder, FILE_NAME);
    }
  }
}
