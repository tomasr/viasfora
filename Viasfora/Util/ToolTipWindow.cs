using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Contracts;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Formatting;
using Winterdom.Viasfora.Text;

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
    private Border holder;
    private int linesDisplayed;

    public ToolTipWindow(ITextView source, ToolTipWindowProvider provider) {
      this.sourceTextView = source;
      this.provider = provider;
    }

    public void SetSize(int widthChars, int heightChars) {
      if ( tipView == null ) {
        CreateTipView();
      }
      double zoom = tipView.ZoomLevel / 100.0;
      double width = zoom * widthChars * this.tipView.FormattedLineSource.ColumnWidth;
      double height = zoom * heightChars * this.tipView.FormattedLineSource.LineHeight;
      this.holder.Width = width + 3;
      this.holder.Height = height + 3;
      this.linesDisplayed = heightChars;
    }

    public UIElement GetWindow(SnapshotPoint bufferPosition) {
      if ( tipView == null ) {
        CreateTipView();
      }
      SnapshotPoint viewPos;
      if ( !RainbowProvider.TryMapToView(this.tipView, bufferPosition, out viewPos) ) {
        return null;
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
      return this.holder;
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
      var roles = this.provider.EditorFactory.CreateTextViewRoleSet("ViasforaToolTip");
      var model = new TipTextViewModel(this.sourceTextView);
      var options = this.provider.OptionsFactory.GlobalOptions;
      this.tipView = this.provider.EditorFactory.CreateTextView(model, roles, options);

      IWpfTextView wpfSource = this.sourceTextView as IWpfTextView;
      if ( wpfSource != null ) {
        this.tipView.ZoomLevel = wpfSource.ZoomLevel;
      }

      this.holder = new Border();
      this.holder.Margin = new Thickness(0);
      this.holder.BorderThickness = new Thickness(0);
      this.holder.Padding = new Thickness(0);
      this.holder.Child = this.tipView.VisualElement;
    }

    public void Dispose() {
      ReleaseView();
      this.sourceTextView = null;
    }

    public void ReleaseView() {
      if ( this.tipView != null ) {
        this.holder.Child = null;
        try {
          this.tipView.Close();
        } catch {
          // swallow exceptions just in case
          // we get disposed after the source view
        }
        this.tipView = null;
      }
      this.holder = null;
    }
    
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
        get { return sourceView.TextViewModel.VisualBuffer; }
      }
      public PropertyCollection Properties {
        get { return this.properties; }
      }

      public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint) {
        return this.sourceView.TextViewModel.GetNearestPointInVisualBuffer(editBufferPoint);
      }

      public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode) {
        return this.sourceView.TextViewModel.GetNearestPointInVisualSnapshot(editBufferPoint, targetVisualSnapshot, trackingMode);
      }

      public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity) {
        return this.sourceView.TextViewModel.IsPointInVisualBuffer(editBufferPoint, affinity);
      }

      public void Dispose() {
      }
    }
  }
}
