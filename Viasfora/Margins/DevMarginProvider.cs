using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Margins {
  [Export(typeof(IWpfTextViewMarginProvider))]
  [Name(Constants.DEV_MARGIN)]
  [Order(After = PredefinedMarginNames.HorizontalScrollBar)]
  [MarginContainer(PredefinedMarginNames.Bottom)]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class DevMarginProvider : IWpfTextViewMarginProvider {
    [Import]
    private IFileExtensionRegistryService ferService = null;
    [Import]
    private IVsfSettings settings = null;
    public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {
      return new DevViewMargin(wpfTextViewHost, ferService, settings);
    }
  }
  
}
