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
  [Export(typeof(IQuickInfoSourceProvider))]
  [Name("Viasfora Xml QuickInfo Provider")]
  [Order(Before = "Default Quick Info Presenter")]
  [ContentType(Constants.CT_XML)]
  [ContentType(Constants.CT_XAML)]
  internal class XmlQuickInfoSourceProvider : IQuickInfoSourceProvider {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
    [Import]
    internal IViewTagAggregatorFactoryService AggregatorFactory { get; set; }

    public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
      return new XmlQuickInfoSource(textBuffer, this);
    }
  }
}
