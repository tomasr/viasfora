using System;

namespace Winterdom.Viasfora {

  static class StringExtensions {
    public static String[] AsList(this String str) {
      return str.Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
  }

}
