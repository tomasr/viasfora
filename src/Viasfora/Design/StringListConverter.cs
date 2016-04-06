using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Winterdom.Viasfora.Design {

  public class StringListConverter : TypeConverter {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
      return sourceType == typeof(String);
    }
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
      return destinationType == typeof(List<String>);
    }
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
      return ((String)value).ToList();
    }
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
      return ((List<String>)value).FromList();
    }
  }
}
