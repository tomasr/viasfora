using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Rainbow {
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(ViewRoles.ToolTipView)]
  public class RainbowTipHighlightProvider : IWpfTextViewCreationListener {

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(RainbowTipHighlight.LAYER)]
    [Order(Before = PredefinedAdornmentLayers.Selection)]
    public AdornmentLayerDefinition HighlightLayer = null;

    [Import]
    public IEditorFormatMapService FormatMapService { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      var formatMap = FormatMapService.GetEditorFormatMap(textView);
      textView.Set(new RainbowTipHighlight(textView, formatMap));
    }
  }

  public class RainbowTipHighlight {
    public const String LAYER = "viasfora.rainbow.tip.highlight";
    public const String TAG = "viasfora.rainbow.tip";
    private IWpfTextView textView;
    private IEditorFormatMap formatMap;
    private IAdornmentLayer layer;

    public RainbowTipHighlight(IWpfTextView textView, IEditorFormatMap formatMap) {
      this.textView = textView;
      this.formatMap = formatMap;
      this.layer = textView.GetAdornmentLayer(LAYER);

      AddHighlight();
      this.textView.Closed += OnViewClosed;
      this.textView.ViewportWidthChanged += OnViewportSizeChanged;
      this.textView.ViewportHeightChanged += OnViewportSizeChanged;
      this.textView.LayoutChanged += OnLayoutChanged;
    }

    private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      this.layer.RemoveAllAdornments();
      AddHighlight();
    }

    private void OnViewportSizeChanged(object sender, EventArgs e) {
      this.layer.RemoveAllAdornments();
      AddHighlight();
    }

    private void OnViewClosed(object sender, EventArgs e) {
      if ( this.textView != null ) {
        this.textView.Closed -= OnViewClosed;
        this.textView.ViewportWidthChanged -= OnViewportSizeChanged;
        this.textView.ViewportHeightChanged -= OnViewportSizeChanged;
        this.textView = null;
        this.formatMap = null;
        this.layer = null;
      }
    }

    private void AddHighlight() {
      // get size
      ViewTipProperty viewTip = this.textView.Get<ViewTipProperty>();
      if ( viewTip == null ) {
        return;
      }
      SnapshotPoint viewPos;
      if ( !RainbowProvider.TryMapToView(this.textView, viewTip.Position, out viewPos) ) {
        return;
      }
      var line = this.textView.TextViewLines.GetTextViewLineContainingBufferPosition(viewPos);
      if ( line == null ) {
        return;
      }
      Rect rc = new Rect(
         new Point(textView.ViewportLeft, line.TextTop),
         new Point(Math.Max(textView.ViewportRight, line.TextRight), line.TextBottom)
      );

      var properties = this.formatMap.GetProperties(Rainbows.TipHilight);
      rc = CreateVisual(line.Extent, rc, properties);
    }

    private Rect CreateVisual(SnapshotSpan span, Rect rc, ResourceDictionary properties) {
      Rectangle highlight = new Rectangle() {
        UseLayoutRounding = true,
        SnapsToDevicePixels = true,
        Fill = new SolidColorBrush((Color)properties["BackgroundColor"]),
        Opacity = 0.10,
        Width = rc.Width,
        Height = rc.Height,
      };

      // Align the image with the top of the bounds of the text geometry
      Canvas.SetLeft(highlight, rc.Left);
      Canvas.SetTop(highlight, rc.Top);

      layer.AddAdornment(
         AdornmentPositioningBehavior.TextRelative, span,
         TAG, highlight, null
      );
      return rc;
    }
  }
}
