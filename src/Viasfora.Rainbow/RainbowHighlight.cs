using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Shapes;

namespace Winterdom.Viasfora.Rainbow {

  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [TextViewRole(ViewRoles.EmbeddedPeekTextView)]
  public class RainbowHilightProvider : IWpfTextViewCreationListener {

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(RainbowHighlight.LAYER)]
    [Order(After = PredefinedAdornmentLayers.Text, Before=AdornmentLayers.InterLine)]
    public AdornmentLayerDefinition HighlightLayer = null;

    [Import]
    public IClassificationFormatMapService FormatMapService { get; set; }
    [Import]
    public IClassificationTypeRegistryService RegistryService { get; set; }
    [Import]
    public IRainbowSettings Settings { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      textView.Set(new RainbowHighlight(textView, this));
    }
    public IClassificationType[] GetRainbowTags() {
      return RainbowColorTagger.GetRainbows(
        RegistryService, Rainbows.MaxDepth
        );
    }
    public IClassificationFormatMap GetFormatMap(ITextView textView) {
      return FormatMapService.GetClassificationFormatMap(textView);
    }
    public int GetRainbowDepth() {
      return this.Settings.RainbowDepth;
    }
  }
  
  public class RainbowHighlight {
    public const String LAYER = "viasfora.rainbow.highlight";
    public const String TAG = "viasfora.rainbow";
    private readonly IAdornmentLayer layer;
    private readonly IWpfTextView view;
    private readonly IClassificationFormatMap formatMap;
    private readonly IClassificationType[] rainbowTags;
    private readonly RainbowHilightProvider provider;

    public RainbowHighlight(
        IWpfTextView textView, 
        RainbowHilightProvider provider
        ) {
      this.view = textView;
      this.provider = provider;
      this.formatMap = provider.GetFormatMap(textView);
      this.rainbowTags = provider.GetRainbowTags();
      layer = view.GetAdornmentLayer(LAYER);
    }

    public static RainbowHighlight Get(ITextView view) {
      return view.Get<RainbowHighlight>();
    }

    public void Start(SnapshotPoint opening, SnapshotPoint closing, int depth) {
      SnapshotSpan span = new SnapshotSpan(opening, closing);
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
        bottom = line.TextBottom + 1;
      } else {
        right = Math.Max(line.Right, this.view.ViewportRight - 1);
        bottom = line.Bottom;
      }
      return new RectangleGeometry(new Rect(left, top, right-left, bottom-top));
    }

    private UIElement MakeAdornment(SnapshotSpan span, Geometry spanGeometry, int depth) {
      var brush = GetRainbowBrush(depth);

      if ( spanGeometry.CanFreeze ) {
        spanGeometry.Freeze();
      }

      return new Path() {
        Data = spanGeometry,
        Stroke = brush,
        StrokeThickness = 1.3,
        Fill = MakeBackgroundBrush(brush),
      };
    }

    private Brush GetRainbowBrush(int depth) {
      var rainbow = rainbowTags[depth % provider.GetRainbowDepth()];
      var properties = formatMap.GetTextProperties(rainbow);
      return properties.ForegroundBrush;
    }

    private Brush MakeBackgroundBrush(Brush brush) {
      SolidColorBrush scb = brush as SolidColorBrush;
      if ( scb == null ) return Brushes.Transparent;
      return new SolidColorBrush(scb.Color) {
        Opacity = 0.10
      };
    }
  }
}
