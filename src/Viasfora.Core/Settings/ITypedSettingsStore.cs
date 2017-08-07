using System;

namespace Winterdom.Viasfora.Settings {
  public interface ITypedSettingsStore : ISettingsStore, IUpdatableSettings {
    String GetString(String name, String defaultValue);
    bool GetBoolean(String name, bool defaultValue);
    int GetInt32(String name, int defaultValue);
    long GetInt64(String name, long defaultValue);
    double GetDouble(String name, double defaultValue);
    T GetEnum<T>(String name, T defaultValue) where T : struct;
    String[] GetList(String name, String[] defaultValue);
    void SetValue(String name, object value);
  }
}
