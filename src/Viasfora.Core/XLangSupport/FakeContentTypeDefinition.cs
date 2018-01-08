using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace Winterdom.Viasfora.XLangSupport {
  internal static class FakeContentTypeDefinitions {
    [Export]
    [Name(ContentTypes.XLang)]
    [BaseDefinition(ContentTypes.Code)]
    internal static ContentTypeDefinition FakeXLang { get; set; }
  }
}
