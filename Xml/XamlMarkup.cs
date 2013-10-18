using System;

namespace Winterdom.Viasfora.Xml {
  class XamlMarkup : IMarkupLanguage {
    public bool IsDelimiter(String tagName) {
      return tagName == "XAML Delimiter";
    }
    public bool IsName(String tagName) {
      return tagName == "XAML Name";
    }
    public bool IsAttribute(String tagName) {
      return tagName == "XAML Attribute";
    }
  }
}
