using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Margins {
  [Export(typeof(IWpfTextViewMarginProvider))]
  [Name(Constants.DEV_MARGIN)]
  [Order(After = PredefinedMarginNames.HorizontalScrollBar)]
  [MarginContainer(PredefinedMarginNames.Bottom)]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class DevMarginProvider : IWpfTextViewMarginProvider {
    [Import]
    private IFileExtensionRegistryService ferService = null;
    public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {
      return new DevViewMargin(wpfTextViewHost, ferService);
    }
  }
  
}
