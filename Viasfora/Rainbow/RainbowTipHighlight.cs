using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Winterdom.Viasfora.Tags;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Rainbow {
  [Export(typeof(IViewTaggerProvider))]
  [ContentType("text")]
  [TextViewRole(ViewRoles.ToolTipView)]
  public class RainbowTipHighlightProvider : IWpfTextViewCreationListener {

    [Export(typeof(AdornmentLayerDefinition))]
    [Name(RainbowTipHighlight.LAYER)]
    [Order(After = PredefinedAdornmentLayers.Text, Before=AdornmentLayers.InterLine)]
    public AdornmentLayerDefinition HighlightLayer = null;

    [Import]
    public IClassificationFormatMapService FormatMapService { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      var formatMap = FormatMapService.GetClassificationFormatMap(textView);
      textView.Set(new RainbowTipHighlight(textView, formatMap));
    }
  }

  public class RainbowTipHighlight {
    public const String LAYER = "viasfora.rainbow.tip.highlight";
    public const String TAG = "viasfora.rainbow.tip";
    private IWpfTextView textView;
    private IClassificationFormatMap formatMap;
    private IAdornmentLayer layer;

    public RainbowTipHighlight(IWpfTextView textView, IClassificationFormatMap formatMap) {
      this.textView = textView;
      this.formatMap = formatMap;
      this.layer = textView.GetAdornmentLayer(LAYER);

      AddHighlight();
      this.textView.Closed += OnViewClosed;
      this.textView.ViewportWidthChanged += OnViewportWidthChanged;
    }

    private void OnViewportWidthChanged(object sender, EventArgs e) {
      this.layer.RemoveAllAdornments();
      AddHighlight();
    }

    private void OnViewClosed(object sender, EventArgs e) {
      if ( this.textView != null ) {
        this.textView.Closed -= OnViewClosed;
        this.textView = null;
        this.formatMap = null;
        this.layer = null;
      }
    }

    private void AddHighlight() {
      // get size
      ViewTipProperty viewTip = this.textView.Get<ViewTipProperty>();
      var line = this.textView.TextViewLines[viewTip.LineNumber];
      Rect rc = new Rect(
         new Point(textView.ViewportLeft, line.TextTop),
         new Point(Math.Max(textView.ViewportRight - 2, line.TextRight), line.TextBottom)
      );

      Rectangle highlight = new Rectangle();
      highlight.Width = rc.Width;
      highlight.Height = rc.Height;

      //Align the image with the top of the bounds of the text geometry
      Canvas.SetLeft(highlight, rc.Left);
      Canvas.SetTop(highlight, rc.Top);

      layer.AddAdornment(
         AdornmentPositioningBehavior.TextRelative, line.Extent,
         TAG, highlight, null
      );
    }
  }
}
