using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Tags;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IViewTaggerProvider))]
  [ContentType(ContentTypes.Text)]
  [TagType(typeof(KeywordTag))]
  public class KeywordTaggerProvider : IViewTaggerProvider {
    [Import]
    public IClassificationTypeRegistryService ClassificationRegistry { get; set; }
    [Import]
    private IClassificationFormatMapService formatService = null;
    [Import]
    public IBufferTagAggregatorFactoryService Aggregator { get; set; }
    [Import]
    public ILanguageFactory LanguageFactory { get; set; }
    [Import]
    public IVsfSettings Settings { get; set; }

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
      var map = formatService.GetClassificationFormatMap(textView);
      var italicsFixer = textView.Properties.GetOrCreateSingletonProperty(
          () => new ItalicsFormatter(map, Settings)
        );
      italicsFixer.AddClassification(Constants.KEYWORD_CLASSIF_NAME);
      italicsFixer.FixIt();
      return new KeywordTagger(buffer, this) as ITagger<T>;
    }

    public KeywordTag GetTag(String name) {
      var type = ClassificationRegistry.GetClassificationType(name);
      return new KeywordTag(type);
    }
  }

  public class ItalicsFormatter {
    private IClassificationFormatMap formatMap;
    private IVsfSettings settings;
    private IList<String> classificationTypes;
    private bool working;

    public ItalicsFormatter(IClassificationFormatMap map, IVsfSettings settings) {
      this.formatMap = map;
      this.settings = settings;
      this.working = false;
      this.classificationTypes = new List<String>();
      this.settings.SettingsChanged += OnSettingsChanged;
      this.formatMap.ClassificationFormatMappingChanged += OnMappingChanged;
    }

    public void AddClassification(String name) {
      this.classificationTypes.Add(name);
    }
    public void FixIt() {
      FixItalics();
    }

    private void OnSettingsChanged(object sender, EventArgs e) {
      FixItalics();
    }

    private void OnMappingChanged(object sender, EventArgs e) {
      FixItalics();
    }

    private void FixItalics() {
      if ( working || formatMap.IsInBatchUpdate ) {
        return;
      }
      working = true;
      formatMap.BeginBatchUpdate();
      try {
        foreach ( var classifierType in formatMap.CurrentPriorityOrder ) {
          if ( classifierType == null ) {
            continue;
          }
          if ( classificationTypes.Contains(classifierType.Classification) )
            SetItalics(classifierType, settings.FlowControlUseItalics);
        }
      } finally {
        formatMap.EndBatchUpdate();
        working = false;
      }
    }
    private void SetItalics(IClassificationType classifierType, bool enable) {
      var tp = formatMap.GetTextProperties(classifierType);

      tp = tp.SetItalic(enable);
      formatMap.SetTextProperties(classifierType, tp);
    }
  }
}
