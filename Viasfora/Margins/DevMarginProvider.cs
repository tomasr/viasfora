using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;

namespace Winterdom.Viasfora.Margins {
  [Export(typeof(IWpfTextViewMarginProvider))]
  [Name("Viasfora Developer Margin")]
  [Order(Before = "Wpf Horizontal Scrollbar")]
  [MarginContainer("bottom")]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  public class DevMarginProvider : IWpfTextViewMarginProvider {
    public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {
      return new DevViewMargin(wpfTextViewHost, marginContainer);
    }
  }
  
  public class DevViewMargin : IWpfTextViewMargin {
    private IWpfTextViewHost wpfTextViewHost;
    private IWpfTextViewMargin marginContainer;
    private IWpfTextView textView;
    private DevMarginVisual visual;
    private DevMarginViewModel model;

    public DevViewMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {
      model = new DevMarginViewModel();
      this.wpfTextViewHost = wpfTextViewHost;
      this.marginContainer = marginContainer;
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
