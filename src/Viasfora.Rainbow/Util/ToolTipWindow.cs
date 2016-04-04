using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora.Util {
  [Export(typeof(IToolTipWindowProvider))]
  public class ToolTipWindowProvider : IToolTipWindowProvider {
    [Import]
    public ITextEditorFactoryService EditorFactory { get; set; }
    [Import]
    public IEditorOptionsFactoryService OptionsFactory { get; set; }

    public IToolTipWindow CreateToolTip(ITextView textView) {
      return new ToolTipWindow(textView, this);
    }
  }

  public class ToolTipWindow : IToolTipWindow {
    private ITextView sourceTextView;
    private ToolTipWindowProvider provider;
    private IWpfTextView tipView;
    private int linesDisplayed;
    private SnapshotPoint pointToDisplay;
    private Border wrapper;
    const double ZoomFactor = 0.80;
    const double WidthFactor = 0.60;

    public ToolTipWindow(ITextView source, ToolTipWindowProvider provider) {
      this.sourceTextView = source;
      this.provider = provider;
    }

    public void SetSize(int widthChars, int heightChars) {
      if ( tipView == null ) {
        CreateTipView();
      }
      double zoom = (tipView.ZoomLevel / 100.0);
      double sourceZoom = this.GetSourceZoomFactor();
      double width = Math.Max(
        sourceZoom * WidthFactor * this.sourceTextView.ViewportWidth,
        zoom * widthChars * this.tipView.FormattedLineSource.ColumnWidth
        );
      double height = zoom * heightChars * this.tipView.LineHeight;
      this.wrapper.Width = width;
      this.wrapper.Height = height;
      this.wrapper.BorderThickness = new Thickness(0);
      this.linesDisplayed = heightChars;
    }

    public object GetWindow(SnapshotPoint bufferPosition) {
      if ( tipView == null ) {
        CreateTipView();
      }
      this.pointToDisplay = bufferPosition;
      this.tipView.Set(new ViewTipProperty(bufferPosition));

      return this.wrapper;
    }

    public void Dispose() {
      ReleaseView();
      this.sourceTextView = null;
    }

    // Delay scrolling the view until it has been sized
    // because otherwise this fails in VS2010 (though
    // works fine in VS2013)
    private void OnViewportWidthChanged(object sender, EventArgs e) {
      this.tipView.ViewportWidthChanged -= this.OnViewportWidthChanged;
      if ( this.tipView.ViewportRight > this.tipView.ViewportLeft ) {
        this.ScrollIntoView(this.pointToDisplay);
      }
    }

    private void ScrollIntoView(SnapshotPoint bufferPosition) {
      SnapshotPoint viewPos;
      if ( !RainbowProvider.TryMapToView(this.tipView, bufferPosition, out viewPos) ) {
        return;
      }
      this.tipView.DisplayTextLineContainingBufferPosition(
        viewPos, this.tipView.LineHeight, ViewRelativePosition.Top
        );
      SetViewportLeft();
      // it could very well be that after this
      // the brace we're interested in isn't visible
      // (it's beyond the viewport right)
      // so let's make it visible
      this.tipView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(viewPos, 1));
    }

    private void SetViewportLeft() {
      double leftMost = 0;
      var lines = this.tipView.TextViewLines;
      // walk all lines from the first visible one
      var line = lines.GetTextViewLineContainingYCoordinate(
        this.tipView.ViewportTop
        );
      int lineNum = lines.IndexOf(line);
      for ( int i = 0; i < linesDisplayed && lineNum + i < lines.Count; i++ ) {
        line = lines[lineNum + i];
        // find the first significant char in the line
        // and check it's left position
        var firstNonWhiteSpace = FindFirstNonWhiteSpaceChar(line);
        if ( firstNonWhiteSpace != null ) {
          var bounds = line.GetCharacterBounds(firstNonWhiteSpace.Value);
          if ( leftMost == 0 || bounds.Left < leftMost ) {
            leftMost = bounds.Left;
          }
        }
      }
      this.tipView.ViewportLeft = leftMost;
    }

    private SnapshotPoint? FindFirstNonWhiteSpaceChar(ITextViewLine line) {
      SnapshotSpan span = line.Extent;
      for ( SnapshotPoint i = span.Start; i < span.End; i += 1 ) {
        if ( !Char.IsWhiteSpace(i.GetChar()) ) {
          return i;
        }
      }
      return null;
    }

    private void CreateTipView() {
      var roles = this.provider.EditorFactory.CreateTextViewRoleSet(ViewRoles.ToolTipView);
      var model = new TipTextViewModel(this.sourceTextView);

      var options = this.provider.OptionsFactory.GlobalOptions;
      this.tipView = this.provider.EditorFactory.CreateTextView(model, roles, options);
      options = this.tipView.Options;
      options.SetOptionValue(DefaultTextViewOptions.IsViewportLeftClippedId, true);
      options.SetOptionValue(ViewOptions.WordWrapStyleId, WordWrapStyles.None);
      options.SetOptionValue(ViewOptions.ViewProhibitUserInput, true);

      this.tipView.ViewportWidthChanged += OnViewportWidthChanged;

      this.tipView.ZoomLevel = GetSourceZoomFactor() * ZoomFactor * 100;
      this.wrapper = new Border();
      this.wrapper.Child = this.tipView.VisualElement;
    }

    private double GetSourceZoomFactor() {
      IWpfTextView wpfSource = this.sourceTextView as IWpfTextView;
      if ( wpfSource != null ) {
        return wpfSource.ZoomLevel / 100;
      } else {
        return 1.0;
      }
    }

    private void ReleaseView() {
      if ( this.tipView != null ) {
        this.tipView.ViewportWidthChanged -= this.OnViewportWidthChanged;
        this.wrapper.Child = null;
        try {
          this.tipView.Close();
        } catch {
          // swallow exceptions just in case
          // we get disposed after the source view
        }
        this.tipView = null;
      }
    }
    
    // TextViewModel for our ToolTip window
    // Notice that we simply return the corresponding buffers
    // from the TextViewModel of the source view
    // Originally, we returned the ViewBuffer as well,
    // but it was causing a NullReferenceException during
    // the initial layout of the window when it was based on
    // a Peek Definition Window coming from metadata 
    // (apparently because the original window has
    // regions collapsed by default).
    // Returning the EditBuffer as the ViewBuffer appears
    // to work around this.
    class TipTextViewModel : ITextViewModel {
      private ITextView sourceView;
      private PropertyCollection properties;

      public TipTextViewModel(ITextView source) {
        this.sourceView = source;
        this.properties = new PropertyCollection();
      }

      public ITextBuffer DataBuffer {
        get { return sourceView.TextViewModel.DataBuffer; }
      }
      public ITextDataModel DataModel {
        get { return sourceView.TextViewModel.DataModel; }
      }
      public ITextBuffer EditBuffer {
        get { return sourceView.TextViewModel.EditBuffer; }
      }
      public ITextBuffer VisualBuffer {
        get { return this.EditBuffer; }
      }
      public PropertyCollection Properties {
        get { return this.properties; }
      }

      public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint) {
        // editBufferPoint MUST be in the editBuffer according to the docs
        if ( editBufferPoint.Snapshot.TextBuffer != this.EditBuffer )
          throw new InvalidOperationException("editBufferPoint is not on the edit buffer");
        return editBufferPoint.TranslateTo(this.EditBuffer.CurrentSnapshot, PointTrackingMode.Positive);
      }

      public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode) {
        // editBufferPoint MUST be in the editBuffer according to the docs
        if ( editBufferPoint.Snapshot.TextBuffer != this.EditBuffer )
          throw new InvalidOperationException("editBufferPoint is not on the edit buffer");
        return editBufferPoint.TranslateTo(targetVisualSnapshot, PointTrackingMode.Positive);
      }

      public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity) {
        // editBufferPoint MUST be in the editBuffer according to the docs
        return editBufferPoint.Snapshot.TextBuffer == this.EditBuffer;
      }

      // Notice we do NOT do anything on the Dispose() call
      // as we don't own the buffers; the source view will
      // dispose them.
      public void Dispose() {
      }
    }
  }
}
