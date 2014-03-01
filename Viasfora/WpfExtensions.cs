using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Winterdom.Viasfora {
  public static class WpfExtensions {
    public static void BindResource(this FrameworkElement element, DependencyProperty property, String key) {
      object resourceKey = VsfPackage.Instance.FindColorResource(key);
      if ( resourceKey != null ) {
        element.SetResourceReference(property, resourceKey);
      }
    }
  }
}
