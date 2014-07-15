using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Formatting;

namespace Winterdom.Viasfora.Text {
  public class CurrentColumnAdornment {
    public const String CUR_COL_TAG = "currentColumn";
    private IAdornmentLayer layer;
    private IWpfTextView view;
    private IClassificationFormatMap formatMap;
    private IClassificationType formatType;
    private Rectangle columnRect;

    public CurrentColumnAdornment(
          IWpfTextView view, IClassificationFormatMap formatMap,
          IClassificationType formatType) {
      this.view = view;
      this.formatMap = formatMap;
      this.formatType = formatType;
      this.columnRect = new Rectangle();
      layer = view.GetAdornmentLayer(Constants.COLUMN_HIGHLIGHT);

      view.Caret.PositionChanged += OnCaretPositionChanged;
      view.ViewportWidthChanged += OnViewportChanged;
      view.ViewportHeightChanged += OnViewportChanged;
      view.LayoutChanged += OnViewLayoutChanged;
      view.TextViewModel.EditBuffer.PostChanged += OnBufferPostChanged;
      view.Closed += OnViewClosed;
      VsfSettings.SettingsUpdated += OnSettingsUpdated;
      formatMap.ClassificationFormatMappingChanged +=
         OnClassificationFormatMappingChanged;

      CreateDrawingObjects();
    }


    void OnSettingsUpdated(object sender, EventArgs e) {
      if ( this.view != null ) {
        CreateDrawingObjects();
        RedrawAdornments();
      }
    }
    void OnClassificationFormatMappingChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        // the user changed something in Fonts and Colors, so
        // recreate our adornments
        CreateDrawingObjects();
        RedrawAdornments();
      }
    }
    void OnViewClosed(object sender, EventArgs e) {
      VsfSettings.SettingsUpdated -= OnSettingsUpdated;
      if ( this.view != null ) {
        view.Caret.PositionChanged -= OnCaretPositionChanged;
        if ( view.TextViewModel != null && view.TextViewModel.EditBuffer != null ) {
          view.TextViewModel.EditBuffer.PostChanged -= OnBufferPostChanged;
        }
        view.ViewportWidthChanged -= OnViewportChanged;
        view.ViewportHeightChanged -= OnViewportChanged;
        view.Closed -= OnViewClosed;
        view.LayoutChanged -= OnViewLayoutChanged;
        view = null;
      }

      if ( this.formatMap != null ) {
        formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
        formatMap = null;
      }
      formatType = null;
    }
    void OnViewportChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        RedrawAdornments();
      }
    }
    void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      if ( e.NewPosition != e.OldPosition && this.view != null ) {
        layer.RemoveAllAdornments();
        this.CreateVisuals(e.NewPosition.VirtualBufferPosition);
      }
    }
    private void OnBufferPostChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        layer.RemoveAllAdornments();
        this.CreateVisuals(this.view.Caret.Position.VirtualBufferPosition);
      }
    }
    private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      if ( this.view != null && e.VerticalTranslation ) {
        layer.RemoveAllAdornments();
        this.CreateVisuals(this.view.Caret.Position.VirtualBufferPosition);
      }
    }

    private void CreateDrawingObjects() {
      // this gets the color settings configured by the
      // user in Fonts and Colors (or the default in out
      // classification type).
      TextFormattingRunProperties format =
         formatMap.GetTextProperties(formatType);

      this.columnRect.StrokeThickness = VsfSettings.HighlightLineWidth;
      this.columnRect.Stroke = format.ForegroundBrush;
      this.columnRect.Fill = format.BackgroundBrush;
    }
    private void RedrawAdornments() {
      if ( view.TextViewLines != null ) {
        layer.RemoveAllAdornments();
        var caret = view.Caret.Position;
        this.CreateVisuals(caret.VirtualBufferPosition);
      }
    }
    private void CreateVisuals(VirtualSnapshotPoint caretPosition) {
      if ( !VsfSettings.CurrentColumnHighlightEnabled ) {
        return; // not enabled
      }
      IWpfTextViewLineCollection textViewLines = view.TextViewLines;
      if ( textViewLines == null )
        return; // not ready yet.
      // make sure the caret position is on the right buffer snapshot
      if ( caretPosition.Position.Snapshot != this.view.TextBuffer.CurrentSnapshot )
        return;

      var line = this.view.GetTextViewLineContainingBufferPosition(
        caretPosition.Position
        );
      var charBounds = line.GetCharacterBounds(caretPosition);

      this.columnRect.Width = charBounds.Width;
      this.columnRect.Height = this.view.ViewportHeight;
      if ( this.columnRect.Height > 2 ) {
        this.columnRect.Height -= 2;
      }

      // Align the image with the top of the bounds of the text geometry
      Canvas.SetLeft(this.columnRect, charBounds.Left);
      Canvas.SetTop(this.columnRect, this.view.ViewportTop);

      layer.AddAdornment(
         AdornmentPositioningBehavior.OwnerControlled, null,
         CUR_COL_TAG, columnRect, null
      );
    }
  }
}
