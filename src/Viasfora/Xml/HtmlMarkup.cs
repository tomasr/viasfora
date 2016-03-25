using System;

namespace Winterdom.Viasfora.Xml {
  class HtmlMarkup : IMarkupLanguage {
    public bool IsDelimiter(String tagName) {
      return tagName == "HTML Tag Delimiter" || tagName == "HTML Operator";
    }
    public bool IsName(String tagName) {
      return tagName == "HTML Element Name";
    }
    public bool IsAttribute(String tagName) {
      return tagName == "HTML Attribute Name";
    }

    public bool IsRazorTag(String tagName) {
      return tagName == "RazorTagHelperElement";
    }
  }
}
