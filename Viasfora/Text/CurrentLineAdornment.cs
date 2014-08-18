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
  public class CurrentLineAdornment {
    public const String CUR_LINE_TAG = "currentLine";
    private IAdornmentLayer layer;
    private IWpfTextView view;
    private IClassificationFormatMap formatMap;
    private IClassificationType formatType;
    private IVsfSettings settings;
    private Rectangle lineRect;

    public CurrentLineAdornment(
          IWpfTextView view, IClassificationFormatMap formatMap,
          IClassificationType formatType, IVsfSettings settings) {
      this.view = view;
      this.formatMap = formatMap;
      this.formatType = formatType;
      this.settings = settings;
      this.layer = view.GetAdornmentLayer(Constants.LINE_HIGHLIGHT);
      this.lineRect = new Rectangle();

      view.Caret.PositionChanged += OnCaretPositionChanged;
      view.ViewportWidthChanged += OnViewportWidthChanged;
      view.LayoutChanged += OnLayoutChanged;
      view.ViewportLeftChanged += OnViewportLeftChanged;
      view.Closed += OnViewClosed;
      view.Options.OptionChanged += OnSettingsChanged;

      this.settings.SettingsChanged += OnSettingsChanged;
      formatMap.ClassificationFormatMappingChanged +=
         OnClassificationFormatMappingChanged;

      CreateDrawingObjects();
    }

    void OnViewClosed(object sender, EventArgs e) {
      if ( this.settings != null ) {
        this.settings.SettingsChanged -= OnSettingsChanged;
        this.settings = null;
      }
      if ( this.view != null ) {
        view.Options.OptionChanged -= OnSettingsChanged;
        view.Caret.PositionChanged -= OnCaretPositionChanged;
        view.ViewportWidthChanged -= OnViewportWidthChanged;
        view.LayoutChanged -= OnLayoutChanged;
        view.ViewportLeftChanged -= OnViewportLeftChanged;
        view.Closed -= OnViewClosed;
        view = null;
      }
      if ( formatMap != null ) {
        formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
        formatMap = null;
      }
      layer = null;
      formatType = null;
    }
    void OnSettingsChanged(object sender, EventArgs e) {
      CreateDrawingObjects();
      RedrawAdornments();
    }
    void OnViewportLeftChanged(object sender, EventArgs e) {
      RedrawAdornments();
    }
    void OnViewportWidthChanged(object sender, EventArgs e) {
      RedrawAdornments();
    }
    void OnClassificationFormatMappingChanged(object sender, EventArgs e) {
      CreateDrawingObjects();
    }
    void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      ITextViewLine newLine = GetLineByPos(e.NewPosition);
      ITextViewLine oldLine = GetLineByPos(e.OldPosition);
      if ( newLine != oldLine ) {
        layer.RemoveAdornmentsByTag(CUR_LINE_TAG);
        this.CreateVisuals(newLine);
      }
    }
    void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      SnapshotPoint caret = view.Caret.Position.BufferPosition;
      foreach ( var line in e.NewOrReformattedLines ) {
        if ( line.ContainsBufferPosition(caret) ) {
          this.CreateVisuals(line);
          break;
        }
      }
    }

    private void CreateDrawingObjects() {
      // this gets the color settings configured by the
      // user in Fonts and Colors (or the default in out
      // classification type).
      TextFormattingRunProperties format =
         formatMap.GetTextProperties(formatType);

      this.lineRect.Fill = format.BackgroundBrush;
      this.lineRect.Stroke = format.ForegroundBrush;
      this.lineRect.StrokeThickness = settings.HighlightLineWidth;
    }
    private void RedrawAdornments() {
      if ( view.TextViewLines != null ) {
        layer.RemoveAllAdornments();
        var caret = view.Caret.Position;
        ITextViewLine line = GetLineByPos(caret);
        this.CreateVisuals(line);
      }
    }
    private ITextViewLine GetLineByPos(CaretPosition pos) {
      SnapshotPoint point = pos.BufferPosition;
      if ( point.Snapshot != view.TextSnapshot ) {
        point = point.TranslateTo(view.TextSnapshot, PointTrackingMode.Positive);
      }
      return view.GetTextViewLineContainingBufferPosition(point);
    }
    private bool IsEnabled() {
      return settings.CurrentLineHighlightEnabled;
    }
    private void CreateVisuals(ITextViewLine line) {
      if ( !IsEnabled() ) {
        return; // not enabled
      }
      IWpfTextViewLineCollection textViewLines = view.TextViewLines;
      if ( textViewLines == null )
        return; // not ready yet.
      SnapshotSpan span = line.Extent;
      Rect rc = new Rect(
         new Point(view.ViewportLeft, line.TextTop),
         new Point(Math.Max(view.ViewportRight - 2, line.TextRight), line.TextBottom)
      );

      lineRect.Width = rc.Width;
      lineRect.Height = rc.Height;

      //Align the image with the top of the bounds of the text geometry
      Canvas.SetLeft(lineRect, rc.Left);
      Canvas.SetTop(lineRect, rc.Top);

      layer.AddAdornment(
         AdornmentPositioningBehavior.TextRelative, span,
         CUR_LINE_TAG, lineRect, null
      );
    }
  }

}
