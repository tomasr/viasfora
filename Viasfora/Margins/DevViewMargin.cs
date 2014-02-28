using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Margins {
  public class DevViewMargin : IWpfTextViewMargin {
    private IWpfTextViewHost wpfTextViewHost;
    private IWpfTextView textView;
    private DevMarginVisual visual;
    private DevMarginViewModel model;

    public DevViewMargin(IWpfTextViewHost wpfTextViewHost) {
      model = new DevMarginViewModel();
      this.wpfTextViewHost = wpfTextViewHost;
      this.visual = new DevMarginVisual(model);
      this.visual.ViewBuffer += OnViewBuffer;
      InitializeTextView();
      RefreshBufferGraphList();
    }

    public FrameworkElement VisualElement {
      get { return this.visual; }
    }

    public bool Enabled {
      get { return true;  }
    }

    public ITextViewMargin GetTextViewMargin(string marginName) {
      if ( marginName == "Viasfora Developer Margin" )
        return this;
      return null;
    }

    public double MarginSize {
      get { return 25;  }
    }

    public void Dispose() {
      if ( this.textView != null ) {
        this.textView.BufferGraph.GraphBuffersChanged -= OnGraphBuffersChanged;
        this.textView.BufferGraph.GraphBufferContentTypeChanged -= OnGraphBufferContentTypeChanged;
        this.textView.Caret.PositionChanged -= OnCaretPositionChanged;
      }
    }

    private void InitializeTextView() {
      this.textView = this.wpfTextViewHost.TextView;
      this.textView.BufferGraph.GraphBuffersChanged += OnGraphBuffersChanged;
      this.textView.BufferGraph.GraphBufferContentTypeChanged += OnGraphBufferContentTypeChanged;
      this.textView.Caret.PositionChanged += OnCaretPositionChanged;
    }

    private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      this.model.BufferPosition = e.NewPosition.BufferPosition.Position;
    }

    private void OnGraphBufferContentTypeChanged(object sender, GraphBufferContentTypeChangedEventArgs e) {
      RefreshBufferGraphList();
    }

    private void OnGraphBuffersChanged(object sender, GraphBuffersChangedEventArgs e) {
      RefreshBufferGraphList();
    }

    private void OnViewBuffer(object sender, EventArgs e) {
      var buffers = this.textView.BufferGraph.GetTextBuffers(b => true);
      int index = 0;
      foreach ( var b in buffers ) {
        if ( index++ == this.model.SelectedBuffer.Index ) {
          MessageBox.Show(b.CurrentSnapshot.GetText());
        }
      }
    }

    private void RefreshBufferGraphList() {
      this.model.RefreshBuffers(this.textView.BufferGraph);
    }
  }
}
