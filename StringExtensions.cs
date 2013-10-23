using System;
using System.Collections.Generic;
using System.Text;

namespace Winterdom.Viasfora {

  static class StringExtensions {
    public static String[] AsList(this String str) {
      if ( String.IsNullOrEmpty(str) ) return null;
      return str.Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
    public static String FromList(this IEnumerable<String> list) {
      StringBuilder sb = new StringBuilder();
      foreach ( String s in list ) {
        if ( sb.Length > 0 ) sb.Append(',');
        sb.Append(s);
      }
      return sb.ToString();
    }
  }

}
