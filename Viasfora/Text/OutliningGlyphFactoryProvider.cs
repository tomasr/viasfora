using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IGlyphFactoryProvider))]
  [ContentType("Text")]
  [TagType(typeof(OutliningGlyphTag))]
  [Name("VsfOutliningGlyph")]
  public class OutliningGlyphFactoryProvider : IGlyphFactoryProvider {
    public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin) {
      return new OutliningGlyphFactory();
    }


    class OutliningGlyphFactory : IGlyphFactory {
      public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag) {
        OutliningGlyphTag ourTag = tag as OutliningGlyphTag;
        if ( ourTag == null ) {
          return null;
        }
        const double minSize = 16.0;
        double size = line != null
          ? Math.Min(minSize, line.TextHeight)
          : minSize;
        // TODO: Implement something remotely useful!
        TextBlock tb = new TextBlock();
        tb.Text = "U";
        tb.FontWeight = FontWeights.Bold;
        tb.Height = tb.Width = minSize;
        tb.TextAlignment = TextAlignment.Center;
        tb.Foreground = Brushes.White;

        Rectangle rect = new Rectangle();
        rect.Height = rect.Width = minSize * 0.9;
        rect.Stroke = Brushes.Transparent;
        rect.Fill = Brushes.BlueViolet;
        rect.RadiusX = rect.RadiusY = rect.Height * 0.1;

        tb.Background = new VisualBrush(rect);
        return tb;
      }
    }
  }
}
