using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
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
    private Brush fillBrush;
    private Pen borderPen;
    private Image currentHighlight = null;

    public CurrentColumnAdornment(
          IWpfTextView view, IClassificationFormatMap formatMap,
          IClassificationType formatType) {
      this.view = view;
      this.formatMap = formatMap;
      this.formatType = formatType;
      layer = view.GetAdornmentLayer(Constants.LINE_HIGHLIGHT);

      view.Caret.PositionChanged += OnCaretPositionChanged;
      view.ViewportWidthChanged += OnViewportChanged;
      view.ViewportHeightChanged += OnViewportChanged;
      view.LayoutChanged += OnLayoutChanged;
      view.Closed += OnViewClosed;
      VsfSettings.SettingsUpdated += OnSettingsUpdated;
      formatMap.ClassificationFormatMappingChanged +=
         OnClassificationFormatMappingChanged;

      CreateDrawingObjects();
    }

    void OnSettingsUpdated(object sender, EventArgs e) {
      this.currentHighlight = null;
      CreateDrawingObjects();
      RedrawAdornments();
    }
    void OnViewClosed(object sender, EventArgs e) {
      view.Caret.PositionChanged -= OnCaretPositionChanged;
      view.ViewportWidthChanged -= OnViewportChanged;
      view.ViewportHeightChanged -= OnViewportChanged;
      view.LayoutChanged -= OnLayoutChanged;
      view.Closed -= OnViewClosed;
      VsfSettings.SettingsUpdated -= OnSettingsUpdated;
    }
    void OnViewportChanged(object sender, EventArgs e) {
      this.currentHighlight = null; // force redraw
      RedrawAdornments();
    }
    void OnClassificationFormatMappingChanged(object sender, EventArgs e) {
      // the user changed something in Fonts and Colors, so
      // recreate our adornments
      this.currentHighlight = null;
      CreateDrawingObjects();
    }
    void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      // TODO: Only redraw if there are changes
      /*if ( e.NewPosition != e.OldPosition )*/ {
        layer.RemoveAdornmentsByTag(CUR_COL_TAG);
        this.CreateVisuals(e.NewPosition.VirtualBufferPosition);
      }
    }
    void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      SnapshotPoint caret = view.Caret.Position.BufferPosition;
      foreach ( var line in e.NewOrReformattedLines ) {
        if ( line.ContainsBufferPosition(caret) ) {
          RedrawAdornments();
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

      fillBrush = format.BackgroundBrush;
      Brush penBrush = format.ForegroundBrush;
      borderPen = new Pen(penBrush, VsfSettings.HighlightLineWidth);
      borderPen.Freeze();
      RedrawAdornments();
    }
    private void RedrawAdornments() {
      if ( view.TextViewLines != null ) {
        if ( currentHighlight != null ) {
          layer.RemoveAdornment(currentHighlight);
        }
        this.currentHighlight = null; // force redraw
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

      double lineWidth = VsfSettings.HighlightLineWidth;
      var line = this.view.GetTextViewLineContainingBufferPosition(
        caretPosition.Position
        );
      var charBounds = line.GetCharacterBounds(caretPosition);
      Rect rc = new Rect(
         new Point(charBounds.Left-lineWidth, this.view.ViewportTop),
         new Point(charBounds.Right, this.view.ViewportBottom-2)
      );

      if ( NeedsNewImage(rc) ) {
        Geometry g = new RectangleGeometry(rc, 0.5, 0.5);
        GeometryDrawing drawing = new GeometryDrawing(fillBrush, borderPen, g);
        drawing.Freeze();
        DrawingImage drawingImage = new DrawingImage(drawing);
        drawingImage.Freeze();
        Image image = new Image();
        // work around WPF rounding bug
        image.UseLayoutRounding = false;
        image.Source = drawingImage;
        currentHighlight = image;
      }

      //Align the image with the top of the bounds of the text geometry
      Canvas.SetLeft(currentHighlight, rc.Left);
      Canvas.SetTop(currentHighlight, rc.Top);

      layer.AddAdornment(
         AdornmentPositioningBehavior.ViewportRelative, null,
         CUR_COL_TAG, currentHighlight, null
      );
    }
    private bool NeedsNewImage(Rect rc) {
      if ( currentHighlight == null )
        return true;
      if ( !AreClose(currentHighlight.Width, rc.Width) )
        return true;
      return !AreClose(currentHighlight.Height, rc.Height);
    }
    private bool AreClose(double d1, double d2) {
      double diff = d1 - d2;
      return Math.Abs(diff) < 0.1;
    }
  }
}
