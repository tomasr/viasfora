using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora {

  public static class VsfSettings {
    const String KEYWORD_CLASSIFIER_ENABLED = "KeywordClassifierEnabled";
    const String XML_EXTENSIONS_ENABLED = "XmlExtensionsEnabled";
    const String CURRENT_LINE_ENABLED = "CurrentLineHighlightEnabled";

    private static VsfSettingsStore settings = new VsfSettingsStore();
    
    public static bool KeywordClassifierEnabled {
      get { return GetBoolean(KEYWORD_CLASSIFIER_ENABLED, true); }
      set { SetValue(KEYWORD_CLASSIFIER_ENABLED, value); }
    }
    public static bool XmlExtensionsEnabled {
      get { return GetBoolean(XML_EXTENSIONS_ENABLED, true); }
      set { SetValue(XML_EXTENSIONS_ENABLED, value); }
    }
    public static bool CurrentLineHighlightEnabled {
      get { return GetBoolean(CURRENT_LINE_ENABLED, false); }
      set { SetValue(CURRENT_LINE_ENABLED, value); }
    }

    public static void Save() {
      settings.Write();
    }

    private static bool GetBoolean(String name, bool defval) {
      String val = settings.Get(name);
      return String.IsNullOrEmpty(val) ? defval : Convert.ToBoolean(defval);
    }

    public static void SetValue(String name, object value) {
      if ( value != null ) {
        settings.Set(name, Convert.ToString(value));
      } else {
        settings.Set(name, null);
      }
    }
 }
}
