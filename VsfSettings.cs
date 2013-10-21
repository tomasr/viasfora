using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora {

  public static class VsfSettings {
    const String KEYWORD_CLASSIFIER_ENABLED = "KeywordClassifierEnabled";
    const String ESCAPE_SEQ_ENABLED = "EscapeSequencesEnabled";
    const String CURRENT_LINE_ENABLED = "CurrentLineHighlightEnabled";
    const String XMLNS_PREFIX_ENABLED = "XmlnsPrefixEnabled";
    const String XML_CLOSE_TAG_ENABLED = "XmlCloseTagEnabled";
    const String XML_MATCH_TAGS_ENABLED = "XmlMatchTagsEnabled";

    private static VsfSettingsStore settings = new VsfSettingsStore();
    
    public static bool KeywordClassifierEnabled {
      get { return GetBoolean(KEYWORD_CLASSIFIER_ENABLED, true); }
      set { SetValue(KEYWORD_CLASSIFIER_ENABLED, value); }
    }
    public static bool EscapeSeqHighlightEnabled {
      get { return GetBoolean(ESCAPE_SEQ_ENABLED, true); }
      set { SetValue(ESCAPE_SEQ_ENABLED, value); }
    }
    public static bool XmlnsPrefixHighlightEnabled {
      get { return GetBoolean(XMLNS_PREFIX_ENABLED, true); }
      set { SetValue(XMLNS_PREFIX_ENABLED, value); }
    }
    public static bool XmlCloseTagHighlightEnabled {
      get { return GetBoolean(XML_CLOSE_TAG_ENABLED, true); }
      set { SetValue(XML_CLOSE_TAG_ENABLED, value); }
    }
    public static bool XmlMatchTagsEnabled {
      get { return GetBoolean(XML_MATCH_TAGS_ENABLED, true); }
      set { SetValue(XML_MATCH_TAGS_ENABLED, value); }
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
      return String.IsNullOrEmpty(val) ? defval : Convert.ToBoolean(val);
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
