using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(PredefinedTextViewRoles.Interactive)]
  internal sealed class CurrentColumnAdornmentFactory : IWpfTextViewCreationListener {
    [Import]
    public IClassificationTypeRegistryService ClassificationRegistry = null;
    [Import]
    public IClassificationFormatMapService FormatMapService = null;
    [Import]
    public IVsfSettings Settings = null;

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(Constants.COLUMN_HIGHLIGHT)]
    [Order(Before = AdornmentLayers.InterLine)]
    public AdornmentLayerDefinition editorAdornmentLayer = null;

    public void TextViewCreated(IWpfTextView textView) {
      IClassificationType classification =
         ClassificationRegistry.GetClassificationType(Constants.COLUMN_HIGHLIGHT);
      IClassificationFormatMap map =
         //FormatMapService.GetClassificationFormatMap(FontsAndColorsCategories.TextEditorCategory);
         FormatMapService.GetClassificationFormatMap(textView);
      textView.Properties.GetOrCreateSingletonProperty(
        () => {
          return new CurrentColumnAdornment(textView, map, classification, Settings);
        });
    }
  }
}
