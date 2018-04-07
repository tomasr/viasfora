using System;
using System.ComponentModel.Composition;

namespace Winterdom.Viasfora.Settings {
  [PartCreationPolicy(CreationPolicy.Shared)]
  [Export(typeof(ITypedSettingsStore))]
  public class TypedSettingsStore : ITypedSettingsStore {
    private ISettingsStore store;
    private IStorageConversions converter;

    public event EventHandler SettingsChanged;

    [ImportingConstructor]
    public TypedSettingsStore(ISettingsStore store, IStorageConversions converter) {
      this.store = store;
      this.converter = converter;
    }

    public String Get(String name) {
      return this.store.Get(name);
    }

    public bool GetBoolean(String name, bool defaultValue) {
      String val = this.store.Get(name);
      return String.IsNullOrEmpty(val) ? defaultValue : this.converter.ToBoolean(val);
    }

    public double GetDouble(String name, double defaultValue) {
      String val = this.store.Get(name);
      return String.IsNullOrEmpty(val) ? defaultValue : this.converter.ToDouble(val);
    }

    public T GetEnum<T>(String name, T defaultValue) where T : struct {
      String val = this.store.Get(name);
      T actual;
      if ( this.converter.ToEnum<T>(val, out actual) ) {
        return actual;
      }
      return defaultValue;
    }

    public int GetInt32(String name, int defaultValue) {
      String val = this.store.Get(name);
      return String.IsNullOrEmpty(val) ? defaultValue : this.converter.ToInt32(val);
    }

    public long GetInt64(string name, long defaultValue) {
      String val = this.store.Get(name);
      return String.IsNullOrEmpty(val) ? defaultValue : this.converter.ToInt64(val);
    }

    public string[] GetList(string name, string[] defaultValue) {
      String value = GetString(name, "");
      if ( String.IsNullOrEmpty(value) ) {
        return defaultValue;
      }
      var list = this.converter.ToList(value);
      return list.Length > 0 ? list : defaultValue; 
    }

    public String GetString(String name, String defValue) {
      String val = this.store.Get(name);
      return String.IsNullOrEmpty(val) ? defValue : val;
    }

    public void SetValue(String name, object value) {
      if ( value != null ) {
        this.store.Set(name, converter.ToString(value));
      } else {
        this.store.Set(name, null);
      }
    }

    public void Set(string name, string value) {
      this.store.Set(name, value);
    }

    public void Load() {
      this.store.Load();
    }

    public void Save() {
      this.store.Save();
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
