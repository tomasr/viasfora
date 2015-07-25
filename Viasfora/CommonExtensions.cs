using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora {
  public static class CommonExtensions {
    public static bool Has<T>(this IPropertyOwner owner) {
      return owner.Properties.ContainsProperty(typeof(T));
    }
    public static T Get<T>(this IPropertyOwner owner) where T : class {
      T t;
      if ( owner.Properties.TryGetProperty(typeof(T), out t) ) {
        return t;
      }
      return null;
    }
    public static void Set<T>(this IPropertyOwner owner, T value) { 
      owner.Properties.AddProperty(typeof(T), value);
    }

    public static bool IsPeekTextWindow(this ITextView textView) {
      return textView.Roles.Contains(ViewRoles.EmbeddedPeekTextView);
    }
  }
}
