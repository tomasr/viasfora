using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {
  [Export(typeof(IIntellisenseControllerProvider))]
  [Name("Viasfora Xml QuickInfo Controller")]
  [ContentType(XmlConstants.CT_XML)]
  [ContentType(XmlConstants.CT_XAML)]
  internal class XmlQuickInfoControllerProvider : IIntellisenseControllerProvider {
    [Import]
    internal IQuickInfoBroker QuickInfoBroker { get; set; }

    public IIntellisenseController TryCreateIntellisenseController(
        ITextView textView, IList<ITextBuffer> subjectBuffers) {
      return new XmlQuickInfoController(textView, subjectBuffers, this);
    }
  }
}
