using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
    private Brush transparentBrush;
    private Pen borderPen;

    public RainbowAdornment(IWpfTextView textView) {
      this.view = textView;
      this.transparentBrush = Brushes.Transparent;
      this.blackoutBrush = Brushes.Tomato;
      this.borderPen = new Pen(this.transparentBrush, 1);
      layer = view.GetAdornmentLayer(LAYER);
    }


    public static RainbowAdornment Get(ITextView view) {
      return view.Properties.GetProperty<RainbowAdornment>(KEY);
    }

    public void Start() {
      DrawAdornment();
    }
    public void Stop() {
      layer.RemoveAllAdornments();
    }

    private void DrawAdornment() {
      Rect outerRect = new Rect(
        0, 0, view.ViewportWidth,
        view.ViewportHeight
        );
      Rect innerRect = new Rect(100, 100, 150, 150);
      GeometryGroup group = new GeometryGroup();
      group.FillRule = FillRule.EvenOdd;
      group.Children.Add(new RectangleGeometry(outerRect));
      group.Children.Add(new RectangleGeometry(innerRect));

      Drawing drawing = new GeometryDrawing(blackoutBrush, borderPen, group);
      DrawingImage drawingImage = new DrawingImage(drawing);
      drawingImage.Freeze();

      Image image = new Image();
      image.Source = drawingImage;

      layer.AddAdornment(
        AdornmentPositioningBehavior.ViewportRelative, null,
        TAG, image, null
        );
    }
  }
}
