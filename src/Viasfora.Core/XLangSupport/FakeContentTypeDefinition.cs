using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.XLangSupport {
  internal static class FakeContentTypeDefinitions {
    [Export]
    [Name(ContentTypes.XLang)]
    [BaseDefinition(ContentTypes.Code)]
    internal static ContentTypeDefinition FakeXLang { get; set; }
  }
}
