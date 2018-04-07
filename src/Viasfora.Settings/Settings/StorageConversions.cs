using System;
using System.ComponentModel.Composition;
using System.Globalization;

namespace Winterdom.Viasfora.Settings {
  [Export(typeof(IStorageConversions))]
  public class StorageConversions : IStorageConversions {
    public bool ToBoolean(String value) {
      return Convert.ToBoolean(value);
    }
    public int ToInt32(String value) {
      int result;
      var styles = NumberStyles.Integer;
      if ( !int.TryParse(value, styles, CultureInfo.InvariantCulture, out result) ) {
        return Convert.ToInt32(value, CultureInfo.CurrentCulture);
      }
      return result;
    }
    public long ToInt64(String value) {
      long result;
      var styles = NumberStyles.Integer;
      if ( !long.TryParse(value, styles, CultureInfo.InvariantCulture, out result) ) {
        return Convert.ToInt64(value, CultureInfo.CurrentCulture);
      }
      return result;
    }
    public double ToDouble(String value) {
      double result;
      var styles = NumberStyles.AllowLeadingWhite
                 | NumberStyles.AllowTrailingWhite
                 | NumberStyles.AllowLeadingSign
                 | NumberStyles.AllowDecimalPoint
                 | NumberStyles.AllowThousands
                 | NumberStyles.AllowExponent;
      if ( !double.TryParse(value, styles, CultureInfo.InvariantCulture, out result) ) {
        return Convert.ToDouble(value, CultureInfo.CurrentCulture);
      }
      return result;
    }
    public bool ToEnum<T>(String value, out T result) where T : struct {
      return Enum.TryParse<T>(value, out result);
    }
    public String[] ToList(String value) {
      return String.IsNullOrEmpty(value) ? new String[0] : value.AsList(); 
    }
    public String ToString(object value) {
      String[] list = value as String[];
      if ( list != null ) {
        return list.FromList();
      }
      return value != null ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;
    }
  }
}
