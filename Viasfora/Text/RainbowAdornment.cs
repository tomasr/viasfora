using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Text {


  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class RainbowAdornmentProvider : IWpfTextViewCreationListener {

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(RainbowAdornment.LAYER)]
    [Order(After = PredefinedAdornmentLayers.Text)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public AdornmentLayerDefinition editorAdornmentLayer = null;

    public void TextViewCreated(IWpfTextView textView) {
      textView.Properties.AddProperty(RainbowAdornment.KEY, new RainbowAdornment(textView));
    }
  }
  
  public class RainbowAdornment {
    public const String LAYER = "viasfora.rainbow.layer";
    public const String TAG = "viasfora.rainbow";
    public static object KEY = typeof(RainbowAdornment);
    private IAdornmentLayer layer;
    private IWpfTextView view;
    private Brush blackoutBrush;
    private Pen borderPen;

    public RainbowAdornment(IWpfTextView textView) {
      this.view = textView;
      this.blackoutBrush = new SolidColorBrush(Colors.White);
      this.borderPen = new Pen(Brushes.Red, 1);
      layer = view.GetAdornmentLayer(LAYER);
    }


    public static RainbowAdornment Get(ITextView view) {
      return view.Properties.GetProperty<RainbowAdornment>(KEY);
    }

    public void Start(SnapshotPoint opening, SnapshotPoint closing) {
      SnapshotSpan span = new SnapshotSpan(opening, closing);
      var lines = this.view.TextViewLines;
      PathGeometry path = BuildSpanGeometry(span);
      path = path.GetOutlinedPathGeometry();
      DrawAdornment(span, path);
    }

    public void Stop() {
      layer.RemoveAllAdornments();
    }

    private PathGeometry BuildSpanGeometry(SnapshotSpan span) {
      PathGeometry path = new PathGeometry();
      path.FillRule = FillRule.Nonzero;
      foreach ( var line in this.view.TextViewLines ) {
        if ( line.Start > span.End ) break;
        if ( line.ContainsBufferPosition(span.Start)
          || line.ContainsBufferPosition(span.End) 
          || line.Start >= span.Start ) {
          var lineGeometry = BuildLineGeometry(span, line);
          if ( !lineGeometry.IsEmpty() ) {
            path.AddGeometry(lineGeometry);
          }
        }
      }
      return path;
    }

    private Geometry BuildLineGeometry(SnapshotSpan span, ITextViewLine line) {
      double left, top, right, bottom;
      if ( line.ContainsBufferPosition(span.Start) ) {
        var bounds = line.GetCharacterBounds(span.Start);
        left = bounds.Left;
        top = bounds.Top;
      } else {
        left = line.Left;
        top = line.Top;
      }
      if ( line.ContainsBufferPosition(span.End) ) {
        var bounds = line.GetCharacterBounds(span.End);
        right = bounds.Right;
        bottom = bounds.Bottom;
      } else {
        right = Math.Max(line.Right, this.view.ViewportRight - 1);
        bottom = line.Bottom;
      }
      return new RectangleGeometry(new Rect(left, top, right-left, bottom-top));
    }

    private void DrawAdornment(SnapshotSpan span, Geometry spanGeometry) {
      GeometryDrawing geometry = new GeometryDrawing(
        Brushes.Transparent, 
        this.borderPen, 
        spanGeometry
        );

      geometry.Freeze();
      DrawingImage drawing = new DrawingImage(geometry);
      drawing.Freeze();

      Image image = new Image();
      image.Source = drawing;
      image.UseLayoutRounding = false;

      var startLine = this.view.GetTextViewLineContainingBufferPosition(span.Start);

      Canvas.SetLeft(image, spanGeometry.Bounds.Left);
      Canvas.SetTop(image, startLine.Top);

      layer.AddAdornment(
        AdornmentPositioningBehavior.TextRelative, span,
        TAG, image, null
        );
    }
  }
}
