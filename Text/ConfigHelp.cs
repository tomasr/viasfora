using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Winterdom.Viasfora.Text {
  static class ConfigHelp {
    private static string REG_KEY =
       "Software\\Winterdom\\Viasfora";
    public static String GetValue(String name, String defValue) {
      using ( RegistryKey key = Registry.CurrentUser.CreateSubKey(REG_KEY) ) {
        String value = key.GetValue(name, defValue) as String;
        if ( String.IsNullOrEmpty(defValue) ) value = defValue;
        return value;
      }
    }
  }
}
