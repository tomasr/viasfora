using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {
  [Export(typeof(IIntellisenseControllerProvider))]
  [Name("Viasfora Xml QuickInfo Controller")]
  [ContentType(Constants.CT_XML)]
  [ContentType(Constants.CT_XAML)]
  internal class XmlQuickInfoControllerProvider : IIntellisenseControllerProvider {
    [Import]
    internal IQuickInfoBroker QuickInfoBroker { get; set; }
    internal ITagAggregator<ClassificationTag> Aggregator { get; set; }

    public IIntellisenseController TryCreateIntellisenseController(
        ITextView textView, IList<ITextBuffer> subjectBuffers) {
      return new XmlQuickInfoController(textView, subjectBuffers, this);
    }
  }
}
