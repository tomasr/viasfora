using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows;
using System.Windows.Media;

namespace Winterdom.Viasfora.Text {
  //
  // Inspired on https://github.com/NoahRic/ItalicComments/blob/master/ViewCreationListener.cs
  //

  [Export(typeof(IWpfTextViewCreationListener))]
  [Name("Viasfora.bold-as-italics")]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class BoldAsItalicsViewListener : IWpfTextViewCreationListener {
    [Import]
    private IClassificationFormatMapService formatService = null;
    public void TextViewCreated(IWpfTextView textView) {
      var formatMap = formatService.GetClassificationFormatMap(textView);
      textView.Properties.GetOrCreateSingletonProperty(
        () => new BoldAsItalicsFormatter(textView, formatMap)
        );
    }
  }

  public class BoldAsItalicsFormatter {
    private IClassificationFormatMap formatMap;
    private bool working = false;
    public BoldAsItalicsFormatter(IWpfTextView textView, IClassificationFormatMap map) {
      this.formatMap = map;
      textView.GotAggregateFocus += OnViewFocus;
      this.formatMap.ClassificationFormatMappingChanged += OnMappingChanged;
    }

    private void OnMappingChanged(object sender, EventArgs e) {
      MakeBoldItalics();
    }

    private void OnViewFocus(object sender, EventArgs e) {
      ((IWpfTextView)sender).GotAggregateFocus -= OnViewFocus;
      MakeBoldItalics();
    }

    private void MakeBoldItalics() {
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
          MakeItalicsIfApplies(classifierType);
        }
      } finally {
        formatMap.EndBatchUpdate();
        working = false;
      }
    }
    private void MakeItalicsIfApplies(IClassificationType classifierType) {
      var tp = formatMap.GetTextProperties(classifierType);
      var font = tp.Typeface;

      // already italic
      if ( tp.Italic || font.Style == FontStyles.Italic ) {
        return;
      }
      if ( !tp.BoldEmpty && tp.Bold ) {
        tp = tp.SetBold(false).SetItalic(true);
        formatMap.SetTextProperties(classifierType, tp);
      }
      if ( font.Weight > FontWeights.Normal ) {
        var newFont = new Typeface(
          font.FontFamily, FontStyles.Normal,
          FontWeights.Normal, FontStretches.Normal);
        tp = tp.SetTypeface(newFont).SetItalic(true);
        formatMap.SetTextProperties(classifierType, tp);
      }
    }
  }
}
