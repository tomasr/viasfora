using System;

namespace Winterdom.Viasfora.Xml {
  interface IMarkupLanguage {
    bool IsDelimiter(String tagName);
    bool IsName(String tagName);
    bool IsAttribute(String tagName);
    bool IsRazorTag(String tagName);
  }
}
