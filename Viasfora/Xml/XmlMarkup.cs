using System;

namespace Winterdom.Viasfora.Xml {
  class XmlMarkup : IMarkupLanguage {
    public bool IsDelimiter(String tagName) {
      return tagName == "XML Delimiter";
    }
    public bool IsName(String tagName) {
      return tagName == "XML Name";
    }
    public bool IsAttribute(String tagName) {
      return tagName == "XML Attribute";
    }
  }
}
