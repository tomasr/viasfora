using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Tags;
using System.Windows.Input;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Outlining {
  [Export(typeof(IGlyphFactoryProvider))]
  [Export(typeof(IGlyphMouseProcessorProvider))]
  [ContentType(ContentTypes.Text)]
  [TagType(typeof(OutliningGlyphTag))]
  [Name("viasfora.outlining.user.glyphs")]
  public class OutliningGlyphFactoryProvider : IGlyphFactoryProvider, IGlyphMouseProcessorProvider {

    [Import]
    internal IBufferTagAggregatorFactoryService aggregatorFactory = null;
    public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin) {
      return new OutliningGlyphFactory();
    }
    public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin) {
      return new GlyphMouseProcessor(
        wpfTextViewHost, margin, 
        this.aggregatorFactory.CreateTagAggregator<IGlyphTag>(wpfTextViewHost.TextView.TextBuffer)
        );
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
        TextBlock tb = CreateGlyphElement(minSize);
        return tb;
      }

      private TextBlock CreateGlyphElement(double minSize) {
        TextBlock tb = new TextBlock {
          Text = "V",
          FontWeight = FontWeights.Bold,
          TextAlignment = TextAlignment.Center,
          VerticalAlignment = VerticalAlignment.Center,
          Foreground = Brushes.White,
          Height = minSize,
          Width = minSize
        };

        Rectangle rect = new Rectangle();
        rect.Height = rect.Width = minSize * 0.9;
        rect.Stroke = Brushes.Transparent;
        rect.Fill = new SolidColorBrush(Colors.DarkOliveGreen);
        rect.RadiusX = rect.RadiusY = rect.Height * 0.1;

        tb.Background = new VisualBrush(rect);
        return tb;
      }
    }

    class GlyphMouseProcessor : MouseProcessorBase {
      private IWpfTextViewHost theHost;
      private IWpfTextViewMargin theMargin;
      private ITagAggregator<IGlyphTag> tagAggregator;
      Point clickPos;
      public GlyphMouseProcessor(IWpfTextViewHost host, IWpfTextViewMargin margin, ITagAggregator<IGlyphTag> aggregator) {
        this.theHost = host;
        this.theMargin = margin;
        this.tagAggregator = aggregator;
        this.theHost.Closed += OnTextViewHostClosed;
      }
      public GlyphMouseProcessor() {
      }
      public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e) {
        this.clickPos = GetLocation(e);
      }
      public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e) {
        Point pos = GetLocation(e);
        var oLine = GetLineAt(this.clickPos);
        var cLine = GetLineAt(pos);
        if ( oLine != cLine || cLine == null ) return;
        // find what outlining regions start on the current line
        // if it's one of ours, remove it.
        SnapshotSpan span = new SnapshotSpan(cLine.Start, cLine.End);
        var spans = new NormalizedSnapshotSpanCollection(span);

        foreach ( var tag in this.tagAggregator.GetTags(spans) ) {
          if ( !(tag.Tag is OutliningGlyphTag) ) continue;
          var tagSpan = tag.GetSpan(cLine.Snapshot);
          if ( tagSpan.IsEmpty ) continue;
          // we might see tags that cover this region, but
          // don't start on the selected line
          if ( TagStartsOnViewLine(tagSpan, cLine) ) {
            RemoveOutlineAt(tagSpan.Start);
            e.Handled = true;
          }
          break;
        }
      }

      private bool TagStartsOnViewLine(SnapshotSpan tagSpan, ITextViewLine viewLine) {
        var tagLine = tagSpan.Start.GetContainingLine();
        var actualViewLine = viewLine.Start.GetContainingLine();
        return tagLine.LineNumber == actualViewLine.LineNumber;
      }

      private void RemoveOutlineAt(SnapshotPoint snapshotPoint) {
        var textBuffer = this.theHost.TextView.TextBuffer;
        IUserOutlining outlining = UserOutliningManager.Get(textBuffer);
        if ( outlining != null ) {
          outlining.RemoveAt(snapshotPoint);
        }
      }
      private ITextViewLine GetLineAt(Point pos) {
        return this.theHost.TextView.TextViewLines.GetTextViewLineContainingYCoordinate(pos.Y);
      }
      private Point GetLocation(MouseButtonEventArgs e) {
        Point location = e.GetPosition(this.theHost.TextView.VisualElement);
        location.X += this.theHost.TextView.ViewportLeft;
        location.Y += this.theHost.TextView.ViewportTop;
        return location;
      }
      private void OnTextViewHostClosed(object sender, EventArgs e) {
        if ( this.tagAggregator != null ) {
          this.tagAggregator.Dispose();
          this.tagAggregator = null;
        }
        this.theHost.Closed -= OnTextViewHostClosed;
      }
    }

  }
}
