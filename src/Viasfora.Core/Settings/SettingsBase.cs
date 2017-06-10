using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Viasfora.Settings {
  public abstract class SettingsBase {
    protected ISettingsStore Store { get; private set; }
    private IStorageConversions converter;

    public SettingsBase(ISettingsStore store, IStorageConversions converter) {
      this.Store = store;
      this.converter = converter;
    }

    public bool GetBoolean(String name, bool defval) {
      String val = Store.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToBoolean(val);
    }

    public int GetInt32(String name, int defval) {
      String val = Store.Get(name);
      return String.IsNullOrEmpty(val) ? defval : converter.ToInt32(val);
    }
    public long GetInt64(String name, long defval) {
      String val = Store.Get(name);
      return String.IsNullOrEmpty(val) ? defval : converter.ToInt64(val);
    }
    public double GetDouble(String name, double defval) {
      String val = Store.Get(name);
      return String.IsNullOrEmpty(val) ? defval : converter.ToDouble(val);
    }
    public T GetEnum<T>(String name, T defval) where T : struct {
      String val = Store.Get(name);
      T actual;
      if ( converter.ToEnum<T>(val, out actual) ) {
        return actual;
      }
      return defval;
    }

    public String GetValue(String name, String defValue) {
      String val = Store.Get(name);
      return String.IsNullOrEmpty(val) ? defValue : val;
    }
    public void SetValue(String name, object value) {
      if ( value != null ) {
        Store.Set(name, converter.ToString(value));
      } else {
        Store.Set(name, null);
      }
    }

  }
}
