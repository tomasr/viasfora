using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(PredefinedTextViewRoles.Editable)]
  internal sealed class CurrentColumnAdornmentFactory : IWpfTextViewCreationListener {
    [Import]
    public IClassificationTypeRegistryService ClassificationRegistry { get; set; } 
    [Import]
    public IClassificationFormatMapService FormatMapService { get; set; }
    [Import]
    public IVsfSettings Settings { get; set; }

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
