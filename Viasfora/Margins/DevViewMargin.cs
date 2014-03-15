using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

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
      VsfSettings.SettingsUpdated += OnSettingsUpdated;

      UpdateVisibility();
      InitializeTextView();
      RefreshBufferGraphList();
    }

    public FrameworkElement VisualElement {
      get { return this.visual; }
    }

    public bool Enabled {
      get { return VsfSettings.DevMarginEnabled;  }
    }

    public ITextViewMargin GetTextViewMargin(string marginName) {
      if ( marginName == Constants.DEV_MARGIN )
        return this;
      return null;
    }

    public double MarginSize {
      get { return this.visual.ActualHeight + 2;  }
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
      var buffer = GetSelectedBuffer();
      OpenBufferInEditor(buffer);
    }

    private void OnSettingsUpdated(object sender, EventArgs e) {
      if ( this.visual != null ) {
        UpdateVisibility();
      }
    }

    private void UpdateVisibility() {
      this.visual.Visibility = this.Enabled
        ? Visibility.Visible
        : Visibility.Collapsed;
    }

    private ITextBuffer GetSelectedBuffer() {
      var buffers = this.textView.BufferGraph.GetTextBuffers(b => true);
      int selectedIndex = this.model.SelectedBuffer.Index;
      foreach ( var b in buffers ) {
        if ( selectedIndex == 0 ) return b;
        selectedIndex--;
      }
      return null;
    }
    private void OpenBufferInEditor(ITextBuffer b) {
      try {
        TextEditor.OpenBufferInPlainTextEditorAsReadOnly(b);
      } catch ( Exception ex ) {
        MessageBox.Show(ex.Message, "Viasfora Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void RefreshBufferGraphList() {
      this.model.RefreshBuffers(this.textView.BufferGraph);
    }
  }
}
