using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Outlining;
using Winterdom.Viasfora.Text;

namespace Viasfora.Tests.Text {

  internal class FakeClassificationType : IClassificationType {
    public string Classification { get; }
    public IEnumerable<IClassificationType> BaseTypes => Array.Empty<IClassificationType>();

    public FakeClassificationType(string name) {
      Classification = name;
    }

    public bool IsOfType(string type) {
      return String.Equals(Classification, type, StringComparison.Ordinal);
    }
  }

  internal class FakeClassificationFormatMap : IClassificationFormatMap {
    private readonly Dictionary<string, TextFormattingRunProperties> properties
      = new Dictionary<string, TextFormattingRunProperties>();
    private readonly List<IClassificationType> priorityOrder
      = new List<IClassificationType>();
    private bool inBatchUpdate;

    public int SetTextPropertiesCallCount { get; set; }
    public TextFormattingRunProperties LastSetProperties { get; private set; }

    public event EventHandler<EventArgs> ClassificationFormatMappingChanged;

    public bool IsInBatchUpdate => inBatchUpdate;

    public ReadOnlyCollection<IClassificationType> CurrentPriorityOrder =>
      new ReadOnlyCollection<IClassificationType>(priorityOrder);

    public TextFormattingRunProperties DefaultTextProperties { get; set; }
      = TextFormattingRunProperties.CreateTextFormattingRunProperties();

    public void AddEntry(IClassificationType type, TextFormattingRunProperties props) {
      properties[type.Classification] = props;
      if ( !priorityOrder.Contains(type) ) {
        priorityOrder.Add(type);
      }
    }

    public TextFormattingRunProperties GetTextProperties(IClassificationType classificationType) {
      return properties.TryGetValue(classificationType.Classification, out var props)
        ? props
        : TextFormattingRunProperties.CreateTextFormattingRunProperties();
    }

    public TextFormattingRunProperties GetExplicitTextProperties(IClassificationType classificationType) {
      return GetTextProperties(classificationType);
    }

    public void SetTextProperties(IClassificationType classificationType, TextFormattingRunProperties props) {
      properties[classificationType.Classification] = props;
      SetTextPropertiesCallCount++;
      LastSetProperties = props;
    }

    public void SetExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties props) {
      SetTextProperties(classificationType, props);
    }

    public void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties props) { }
    public void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties props, IClassificationType after) { }

    public void SwapPriorities(IClassificationType first, IClassificationType second) { }

    public string GetEditorFormatMapKey(IClassificationType classificationType) {
      return classificationType?.Classification ?? string.Empty;
    }

    public void BeginBatchUpdate() { inBatchUpdate = true; }
    public void EndBatchUpdate() { inBatchUpdate = false; }

    public void RaiseChanged() {
      ClassificationFormatMappingChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  internal class FakeVsfSettings : IVsfSettings {
    public event EventHandler SettingsChanged;
    public bool FlowControlUseItalics { get; set; }
    public bool BoldAsItalicsEnabled { get; set; }
    public bool KeywordClassifierEnabled { get; set; }
    public bool FlowControlKeywordsEnabled { get; set; }
    public bool VisibilityKeywordsEnabled { get; set; }
    public bool QueryKeywordsEnabled { get; set; }
    public bool EscapeSequencesEnabled { get; set; }
    public bool CurrentLineHighlightEnabled { get; set; }
    public bool CurrentColumnHighlightEnabled { get; set; }
    public ColumnStyle CurrentColumnHighlightStyle { get; set; }
    public double HighlightLineWidth { get; set; }
    public bool PresentationModeEnabled { get; set; }
    public int PresentationModeDefaultZoom { get; set; }
    public int PresentationModeEnabledZoom { get; set; }
    public bool PresentationModeIncludeEnvFonts { get; set; }
    public bool ModelinesEnabled { get; set; }
    public int ModelinesNumLines { get; set; }
    public bool DeveloperMarginEnabled { get; set; }
    public AutoExpandMode AutoExpandRegions { get; set; }
    public string TextObfuscationRegexes { get; set; }
    public bool TelemetryEnabled { get; set; }
    public void Load() { }
    public void Save() { }
    public void RaiseSettingsChanged() {
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
