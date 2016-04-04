using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
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
          () => new ItalicsFormatter(textView, map, Settings)
        );
      italicsFixer.AddClassification(Constants.KEYWORD_CLASSIF_NAME);
      return new KeywordTagger(buffer, this) as ITagger<T>;
    }

    public KeywordTag GetTag(String name) {
      var type = ClassificationRegistry.GetClassificationType(name);
      return new KeywordTag(type);
    }
  }

  public class ItalicsFormatter {
    private ITextView textView;
    private IClassificationFormatMap formatMap;
    private IVsfSettings settings;
    private IList<String> classificationTypes;
    private bool working;

    public ItalicsFormatter(ITextView textView, IClassificationFormatMap map, IVsfSettings settings) {
      this.textView = textView;
      this.formatMap = map;
      this.settings = settings;
      this.working = false;
      this.classificationTypes = new List<String>();
      this.settings.SettingsChanged += OnSettingsChanged;
      this.formatMap.ClassificationFormatMappingChanged += OnMappingChanged;
      // Delay activating until after the textView
      // gets focus. Otherwise, VS may crash/hang
      this.textView.GotAggregateFocus += OnTextViewFocus;
    }

    private void OnTextViewFocus(object sender, EventArgs e) {
      this.textView.GotAggregateFocus -= OnTextViewFocus;
      FixIt();
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
