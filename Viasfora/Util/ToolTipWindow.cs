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

    public ToolTipWindow(ITextView source, ToolTipWindowProvider provider) {
      this.sourceTextView = source;
      this.provider = provider;
    }

    public void SetSize(int widthChars, int heightChars) {
      if ( tipView == null ) {
        CreateTipView();
      }
      double width = widthChars * this.tipView.FormattedLineSource.ColumnWidth;
      double height = heightChars * this.tipView.FormattedLineSource.LineHeight;
      this.holder.Width = width + 10;
      this.holder.Height = height + 10;
    }

    public UIElement GetWindow(SnapshotSpan span) {
      if ( tipView == null ) {
        CreateTipView();
      }
      SnapshotSpan viewSpan = MapSpanToView(span);
      this.tipView.ViewScroller.EnsureSpanVisible(
        viewSpan, EnsureSpanVisibleOptions.AlwaysCenter
        );
      SetViewportLeft(viewSpan);
      return this.holder;
    }

    private void SetViewportLeft(SnapshotSpan viewSpan) {
      double leftMost = 0;
      var line = this.tipView.TextViewLines.GetTextViewLineContainingBufferPosition(viewSpan.Start);
      while ( line.VisibilityState != VisibilityState.Hidden ) {
        var firstNonWhiteSpace = FindFirstNonWhiteSpaceChar(line);
        if ( firstNonWhiteSpace == null ) continue;
        var bounds = line.GetCharacterBounds(firstNonWhiteSpace.Value);
        if ( leftMost == 0 || bounds.Left < leftMost ) {
          leftMost = bounds.Left;
        }
        if ( line.IsLastTextViewLineForSnapshotLine ) {
          break;
        }
      }
      this.tipView.ViewScroller.ScrollViewportHorizontallyByPixels(leftMost);
    }

    private SnapshotPoint? FindFirstNonWhiteSpaceChar(IWpfTextViewLine line) {
      SnapshotSpan span = line.Extent;
      for ( SnapshotPoint i = span.Start; i < span.End; i += 1 ) {
        if ( !Char.IsWhiteSpace(i.GetChar()) ) {
          return i;
        }
      }
      return null;
    }

    private SnapshotSpan MapSpanToView(SnapshotSpan span) {
      var spanCollection = this.tipView.BufferGraph.MapUpToBuffer(
        span, SpanTrackingMode.EdgePositive,
        this.tipView.TextSnapshot.TextBuffer);
      return spanCollection.First();
    }


    private void CreateTipView() {
      var roles = this.provider.EditorFactory.CreateTextViewRoleSet("ViasforaToolTip");
      var model = new TipTextViewModel(this.sourceTextView);
      var options = this.provider.OptionsFactory.GlobalOptions;
      this.tipView = this.provider.EditorFactory.CreateTextView(model, roles, options);

      this.holder = new Border();
      this.holder.Child = this.tipView.VisualElement;
    }

    public void Dispose() {
      if ( this.tipView != null ) {
        this.holder.Child = null;
        this.tipView.Close();
        this.tipView = null;
      }
      this.sourceTextView = null;
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
