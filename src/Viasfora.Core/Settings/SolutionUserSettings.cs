using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Settings {
  public class SolutionUserSettings : ISolutionUserSettings {
    private IPersistSettings persistSettings;
    private Encoding encoding;
    public const String DEFAULT_FILE = "__";

    public SolutionUserSettings(IPersistSettings persister) {
      this.persistSettings = persister;
      this.encoding = Encoding.UTF8;
    }

    public void Store<T>(string filePath, T settingsObject) where T : ISettingsObject {
      if ( String.IsNullOrEmpty(filePath) ) {
        filePath = DEFAULT_FILE;
      }
      var json = TryRead();
      if ( json == null ) {
        json = new JObject();
      }
      SetEntry(json, filePath, settingsObject);

      using ( var sw = new StringWriter() ) {
        using ( var jw = new JsonTextWriter(sw) ) {
          jw.Formatting = Formatting.Indented;
          json.WriteTo(jw);
        }
        this.persistSettings.Write(this.encoding.GetBytes(sw.ToString()));
      }
    }

    public T Load<T>(string filePath) where T : ISettingsObject, new() {
      if ( String.IsNullOrEmpty(filePath) ) {
        filePath = DEFAULT_FILE;
      }
      var json = TryRead();
      if ( json == null ) {
        return default(T);
      }
      return GetEntry<T>(json, filePath);
    }


    private T GetEntry<T>(JObject json, String key) where T : ISettingsObject, new() {
      var keyEntry = json[key.ToLowerInvariant()];
      if ( keyEntry == null ) {
        return default(T);
      }
      T settingsObj = new T();
      var settingsEntry = keyEntry[settingsObj.Name];
      if ( settingsEntry == null ) {
        return default(T);
      }
      DeserializeEntry((JObject)settingsEntry, settingsObj);
      return settingsObj;
    }

    private void DeserializeEntry<T>(JObject settingsEntry, T settingsObj) where T: ISettingsObject {
      String text = settingsEntry.ToString();
      using ( var jr = new JsonTextReader(new StringReader(text)) ) {
        settingsObj.Read(jr);
      }
    }
    private void SetEntry<T>(JObject json, string key, T settingsObject) where T : ISettingsObject {
      var keyEntry = json[key.ToLowerInvariant()];
      if ( keyEntry == null ) {
        keyEntry = new JObject();
        json[key.ToLowerInvariant()] = keyEntry;
      }
      var entryObject = SerializeEntry(settingsObject);
      keyEntry[settingsObject.Name] = entryObject;
    }

    private JToken SerializeEntry<T>(T settingsObject) where T : ISettingsObject {
      StringWriter sw = new StringWriter();
      using ( var jw = new JsonTextWriter(sw) ) {
        settingsObject.Save(jw);
      }
      return JObject.Parse(sw.ToString());
    }

    private JObject TryRead() {
      try {
        byte[] data = this.persistSettings.Read();
        if ( data != null && data.Length > 0 ) {
          return JObject.Parse(this.encoding.GetString(data));
        }
      } catch ( Exception ex ) {
        // avoid generating a VS error if 
        // the JSON stored is invalid.
        // See https://github.com/tomasr/viasfora/issues/112
        PkgSource.LogError("Error loading solution user settings", ex);
      }
      return null;
    }
  }
}
