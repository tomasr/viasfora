using System;

namespace Winterdom.Viasfora.Settings {
  public interface IStorageConversions {
    bool ToBoolean(String value);
    int ToInt32(String value);
    long ToInt64(String value);
    double ToDouble(String value);
    bool ToEnum<T>(String value, out T result) where T : struct;
    String[] ToList(String value);
    String ToString(object value);
  }
}
