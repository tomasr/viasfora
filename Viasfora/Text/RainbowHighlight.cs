using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [TextViewRole(ViewRoles.EmbeddedPeekTextView)]
  public class RainbowHilightProvider : IWpfTextViewCreationListener {

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(RainbowHighlight.LAYER)]
    [Order(After = PredefinedAdornmentLayers.Text, Before=AdornmentLayers.InterLine)]
    public AdornmentLayerDefinition HighlightLayer = null;

    [Import]
    private IClassificationFormatMapService formatMapService = null;
    [Import]
    private IClassificationTypeRegistryService registryService = null;

    public void TextViewCreated(IWpfTextView textView) {
      var rainbowTags = RainbowColorTagger.GetRainbows(
        registryService, Constants.MAX_RAINBOW_DEPTH
        );
      var formatMap = formatMapService.GetClassificationFormatMap(textView);
      textView.Set(new RainbowHighlight(textView, formatMap, rainbowTags));
    }
  }
  
  public class RainbowHighlight {
    public const String LAYER = "viasfora.rainbow.highlight";
    public const String TAG = "viasfora.rainbow";
    private readonly IAdornmentLayer layer;
    private readonly IWpfTextView view;
    private readonly IClassificationFormatMap formatMap;
    private readonly IClassificationType[] rainbowTags;

    public RainbowHighlight(
        IWpfTextView textView, 
        IClassificationFormatMap map, 
        IClassificationType[] rainbowTags) {
      this.view = textView;
      this.formatMap = map;
      this.rainbowTags = rainbowTags;
      layer = view.GetAdornmentLayer(LAYER);
    }


    public static RainbowHighlight Get(ITextView view) {
      return view.Get<RainbowHighlight>();
    }

    public void Start(SnapshotPoint opening, SnapshotPoint closing, int depth) {
      SnapshotSpan span = new SnapshotSpan(opening, closing);
      var lines = this.view.TextViewLines;
      PathGeometry path = BuildSpanGeometry(span);
      path = path.GetOutlinedPathGeometry();

      var adornment = MakeAdornment(span, path, depth);
      layer.AddAdornment(
        AdornmentPositioningBehavior.TextRelative, span,
        TAG, adornment, null
        );
    }

    public void Stop() {
      layer.RemoveAllAdornments();
    }

    private PathGeometry BuildSpanGeometry(SnapshotSpan span) {
      PathGeometry path = new PathGeometry();
      path.FillRule = FillRule.Nonzero;
      foreach ( var line in this.view.TextViewLines ) {
        if ( line.Start > span.End ) break;
        if ( LineIntersectsSpan(line, span) ) {
          var lineGeometry = BuildLineGeometry(span, line);
          path.AddGeometry(lineGeometry);
        }
      }
      return path;
    }

    private bool LineIntersectsSpan(ITextViewLine line, SnapshotSpan span) {
      return line.ContainsBufferPosition(span.Start)
          || line.ContainsBufferPosition(span.End)
          || line.Start >= span.Start;
    }

    private Geometry BuildLineGeometry(SnapshotSpan span, ITextViewLine line) {
      double left, top, right, bottom;
      if ( line.ContainsBufferPosition(span.Start) ) {
        var bounds = line.GetCharacterBounds(span.Start);
        left = bounds.Left;
        top = line.TextTop;
      } else {
        left = line.Left;
        top = line.Top;
      }
      if ( line.ContainsBufferPosition(span.End) ) {
        var bounds = line.GetCharacterBounds(span.End);
        right = bounds.Right;
        bottom = line.TextBottom;
      } else {
        right = Math.Max(line.Right, this.view.ViewportRight - 1);
        bottom = line.Bottom;
      }
      return new RectangleGeometry(new Rect(left, top, right-left, bottom-top));
    }

    private UIElement MakeAdornment(SnapshotSpan span, Geometry spanGeometry, int depth) {
      var brush = GetRainbowBrush(depth);

      GeometryDrawing geometry = new GeometryDrawing(
        MakeBackgroundBrush(brush),
        new Pen(brush, 1),
        spanGeometry
        );

      if ( geometry.CanFreeze ) geometry.Freeze();
      DrawingImage drawing = new DrawingImage(geometry);
      if ( drawing.CanFreeze ) drawing.Freeze();

      Image image = new Image {
        Source = drawing,
        UseLayoutRounding = false
      };

      Canvas.SetLeft(image, spanGeometry.Bounds.Left);
      Canvas.SetTop(image, spanGeometry.Bounds.Top);

      return image;
    }

    private Brush GetRainbowBrush(int depth) {
      var rainbow = rainbowTags[depth % rainbowTags.Length];
      var properties = formatMap.GetTextProperties(rainbow);
      return properties.ForegroundBrush;
    }

    private Brush MakeBackgroundBrush(Brush brush) {
      SolidColorBrush scb = brush as SolidColorBrush;
      if ( scb == null ) return Brushes.Transparent;
      Brush newBrush = new SolidColorBrush(scb.Color);
      newBrush.Opacity = 0.05;
      return newBrush;
    }
  }
}
