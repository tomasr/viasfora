using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {
  [Export(typeof(IAsyncQuickInfoSourceProvider))]
  [Name("Viasfora Xml QuickInfo Provider")]
  [Order(Before = "Default Quick Info Presenter")]
  [ContentType(XmlConstants.CT_XML)]
  [ContentType(XmlConstants.CT_XAML)]
  internal class XmlQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
    [Import]
    internal IViewTagAggregatorFactoryService AggregatorFactory { get; set; }

    public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
      return new XmlQuickInfoSource(textBuffer, this);
    }
  }
}
