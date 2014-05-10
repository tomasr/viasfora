using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  internal sealed class CurrentColumnAdornmentFactory : IWpfTextViewCreationListener {
    [Import]
    public IClassificationTypeRegistryService ClassificationRegistry = null;
    [Import]
    public IClassificationFormatMapService FormatMapService = null;

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(Constants.COLUMN_HIGHLIGHT)]
    [Order(Before = AdornmentLayers.InterLine)]
    public AdornmentLayerDefinition editorAdornmentLayer = null;

    public void TextViewCreated(IWpfTextView textView) {
      IClassificationType classification =
         ClassificationRegistry.GetClassificationType(Constants.COLUMN_HIGHLIGHT);
      IClassificationFormatMap map =
         FormatMapService.GetClassificationFormatMap(textView);
      textView.Properties.GetOrCreateSingletonProperty(
        () => {
          return new CurrentColumnAdornment(textView, map, classification);
        });
    }
  }
}
