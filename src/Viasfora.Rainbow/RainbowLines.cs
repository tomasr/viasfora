using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Winterdom.Viasfora.Rainbow {
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType(ContentTypes.Text)]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [TextViewRole(ViewRoles.EmbeddedPeekTextView)]
  public class RainbowLinesProvider : IWpfTextViewCreationListener {
    [Export(typeof(AdornmentLayerDefinition))]
    [Name(RainbowLines.LAYER)]
    [Order(After = PredefinedAdornmentLayers.Text, Before=AdornmentLayers.InterLine)]
    public AdornmentLayerDefinition LinesLayer = null;

    [Import]
    public IClassificationFormatMapService FormatMapService { get; set; }
    [Import]
    public IClassificationTypeRegistryService RegistryService { get; set; }
    [Import]
    public IRainbowSettings Settings { get; set; }

    public void TextViewCreated(IWpfTextView textView) {
      textView.Set(new RainbowLines(textView, this));
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

  public class RainbowLines {
    public const String LAYER = "viasfora.rainbow.lines";
    public const String TAG = "viasfora.rainbow";
    private IWpfTextView view;
    private readonly IAdornmentLayer layer;
    private readonly IClassificationFormatMap formatMap;
    private readonly IClassificationType[] rainbowTags;
    private readonly RainbowLinesProvider provider;

    public RainbowLines(
        IWpfTextView textView, 
        RainbowLinesProvider provider
        ) {
      this.view = textView;
      this.provider = provider;
      this.formatMap = provider.GetFormatMap(textView);
      this.rainbowTags = provider.GetRainbowTags();
      layer = view.GetAdornmentLayer(LAYER);

      this.provider.Settings.SettingsChanged += OnSettingsChanged;
      this.view.Caret.PositionChanged += OnCaretPositionChanged;
      this.view.Options.OptionChanged += OnOptionsChanged;
      this.view.LayoutChanged += OnLayoutChanged;
      this.view.ViewportLeftChanged += OnViewportLeftChanged;
      this.view.ViewportWidthChanged += OnViewportWidthChanged;
      this.view.ViewportHeightChanged += OnViewportHeightChanged;
      this.view.Closed += OnViewClosed;
    }

    private void OnViewClosed(object sender, EventArgs e) {
      if ( this.view != null ) {
        this.view.Options.OptionChanged -= OnOptionsChanged;
        this.view.Caret.PositionChanged -= OnCaretPositionChanged;
        this.view.LayoutChanged -= OnLayoutChanged;
        this.view.ViewportWidthChanged -= OnViewportWidthChanged;
        this.view.ViewportHeightChanged -= OnViewportHeightChanged;
        this.view.ViewportLeftChanged -= OnViewportLeftChanged;
        this.view.Closed -= OnViewClosed;
        this.view = null;
      }
    }

    private void OnSettingsChanged(object sender, EventArgs e) {
      layer.RemoveAllAdornments();
      var bufferPos = GetPosition(this.view.Caret.Position.BufferPosition);
      RedrawVisuals(bufferPos);
    }

    private void OnOptionsChanged(object sender, EditorOptionChangedEventArgs e) {
      layer.RemoveAllAdornments();
      var bufferPos = GetPosition(this.view.Caret.Position.BufferPosition);
      RedrawVisuals(bufferPos);
    }

    private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      RedrawVisuals(GetPosition(e.NewPosition.BufferPosition));
    }

    private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      var bufferPos = GetPosition(this.view.Caret.Position.BufferPosition);
      RedrawVisuals(bufferPos);
    }

    private void OnViewportHeightChanged(object sender, EventArgs e) {
      var bufferPos = GetPosition(this.view.Caret.Position.BufferPosition);
      RedrawVisuals(bufferPos);
    }

    private void OnViewportLeftChanged(object sender, EventArgs e) {
      var bufferPos = GetPosition(this.view.Caret.Position.BufferPosition);
      RedrawVisuals(bufferPos);
    }

    private void OnViewportWidthChanged(object sender, EventArgs e) {
      var bufferPos = GetPosition(this.view.Caret.Position.BufferPosition);
      RedrawVisuals(bufferPos);
    }

    private SnapshotPoint GetPosition(SnapshotPoint position) {
      if ( position.Snapshot != this.view.TextSnapshot ) {
        return position.TranslateTo(this.view.TextSnapshot, PointTrackingMode.Positive);
      }
      return position;
    }

    private void RedrawVisuals(SnapshotPoint caret) {
      if ( !this.provider.Settings.RainbowLinesEnabled ) {
        return;
      }
      var provider = caret.Snapshot.TextBuffer.Get<RainbowProvider>();
      if ( provider == null ) {
        return;
      }
      var braces = provider.BufferBraces.GetBracePairFromPosition(caret, RainbowHighlightMode.TrackInsertionPoint);
      if ( braces == null ) return;

      SnapshotPoint opening = braces.Item1.ToPoint(caret.Snapshot);
      SnapshotPoint closing = braces.Item2.ToPoint(caret.Snapshot);

      var newSpan = new SnapshotSpan(opening, closing);

      if ( RainbowProvider.TryMapToView(view, opening, out opening)
        && RainbowProvider.TryMapToView(view, closing, out closing) ) {

        var path = CreateVisuals(opening, closing, caret);
        layer.RemoveAllAdornments();
        if ( path != null ) {
          var adornment = MakeAdornment(newSpan, path, braces.Item1.Depth);
          layer.AddAdornment(
            AdornmentPositioningBehavior.OwnerControlled, newSpan,
            TAG, adornment, null
            );
        }
      }
    }

    private UIElement MakeAdornment(SnapshotSpan span, Geometry spanGeometry, int depth) {
      var brush = GetRainbowBrush(depth);

      if ( spanGeometry.CanFreeze ) {
        spanGeometry.Freeze();
      }

      return new Path() {
        Data = spanGeometry,
        Stroke = brush,
        StrokeThickness = 1.2,
      };
    }

    // do we want to cache this?
    private Brush GetRainbowBrush(int depth) {
      var rainbow = rainbowTags[depth % provider.GetRainbowDepth()];
      var properties = formatMap.GetTextProperties(rainbow);
      return properties.ForegroundBrush;
    }

    private Geometry CreateVisuals(SnapshotPoint opening, SnapshotPoint closing, SnapshotPoint caret) {
      var openLine = this.view.TextViewLines.GetTextViewLineContainingBufferPosition(opening);
      var closeLine = this.view.TextViewLines.GetTextViewLineContainingBufferPosition(closing);

      IList<LinePoint> points = null;
      if ( openLine != null && closeLine != null && openLine.Extent == closeLine.Extent ) {
        points = SingleLineSpan(opening, closing);
      } else {
        points = MultiLineSpan(opening, closing);
      }
      if ( points.Count == 0 ) {
        return null;
      }
      PathGeometry geometry = new PathGeometry();
      LinePoint p = points[0];
      for ( int i=1; i < points.Count; i++ ) {
        var p1 = points[i];
        if ( !p.SkipNext ) {
          geometry.AddGeometry(new LineGeometry(p.Point(), p1.Point()));
        }
        p = p1;
      }
      return geometry;
    }

    private IList<LinePoint> MultiLineSpan(SnapshotPoint opening, SnapshotPoint closing) {
      var indent = CalculateLeftOfFirstChar(opening, this.view.FormattedLineSource);
      var lines = this.view.TextViewLines.GetTextViewLinesIntersectingSpan(new SnapshotSpan(opening, closing));

      // figure out where the vertical line goes
      // ViewportLeft is substracted here so that if the view is scrolled
      // to the right, we don't draw the line visible if it should be hidden.
      var guidelineX = (indent + (this.view.FormattedLineSource.ColumnWidth / 2) + 2)
                     - this.view.ViewportLeft;

      List<LinePoint> points = new List<LinePoint>();
      for ( int i=0; i < lines.Count; i++ ) {
        var line = lines[i];
        if ( i == 0 && line.ContainsBufferPosition(opening) ) {
          // first line contains opening char
          var openBounds = line.GetCharacterBounds(opening);
          if ( openBounds.Left > guidelineX ) {
            // draw line from brace to guideline position
            points.Add(new LinePoint(openBounds.Right, line.Bottom));
          }
          points.Add(new LinePoint(guidelineX, line.Bottom));
        } else {
          // regular line in between
          var xPos = line.GetBufferPositionFromXCoordinate(guidelineX);
          if (xPos.HasValue && !Char.IsWhiteSpace(xPos.Value.GetChar()) ) {
            // add a hole in the line to avoid running it over text
            points.Add(new LinePoint(guidelineX, line.Top, true));
          }
          points.Add(new LinePoint(guidelineX, line.Bottom));
          if ( i == lines.Count - 1 && line.ContainsBufferPosition(closing) ) {
            // last line contains the closing element and is visible
            var closeBounds = line.GetCharacterBounds(closing);
            if ( closeBounds.Right < guidelineX || closeBounds.Left > guidelineX ) {
              points.Add(new LinePoint(closeBounds.Left, line.Bottom));
            }
          }
        }
      }
      return points;
    }

    private double CalculateLeftOfFirstChar(SnapshotPoint open, IFormattedLineSource fls) {
      var line = open.GetContainingLine();
      var x = this.view.ViewportLeft;
      var start = line.Start;
      while ( Char.IsWhiteSpace(start.GetChar()) ) {
        x += start.GetChar() == '\t' ? fls.TabSize * fls.ColumnWidth : fls.ColumnWidth;
        start = start + 1;
      }
      return x;
    }

    private IList<LinePoint> SingleLineSpan(SnapshotPoint opening, SnapshotPoint closing) {
      var startb = this.view.TextViewLines.GetCharacterBounds(opening);
      var endb = this.view.TextViewLines.GetCharacterBounds(closing);

      return new List<LinePoint> {
        new LinePoint(startb.Left, startb.Bottom),
        new LinePoint(endb.Right, endb.Bottom)
      };
    }

    struct LinePoint {
      public readonly double X;
      public readonly double Y;
      public readonly bool SkipNext;

      public LinePoint(double x, double y, bool skipNext = false) {
        this.X = x;
        this.Y = y;
        this.SkipNext = skipNext;
      }

      public Point Point() => new Point(X, Y);
    }
  }
}
