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
    private IFileExtensionRegistryService extensionRegistry;
    private IWpfTextViewHost wpfTextViewHost;
    private IWpfTextView textView;
    private DevMarginVisual visual;
    private DevMarginViewModel model;

    public DevViewMargin(IWpfTextViewHost wpfTextViewHost, IFileExtensionRegistryService fers) {
      model = new DevMarginViewModel();
      this.wpfTextViewHost = wpfTextViewHost;
      this.extensionRegistry = fers;
      this.visual = new DevMarginVisual(model);
      this.visual.ViewBuffer += OnViewBuffer;
      VsfSettings.SettingsUpdated += OnSettingsUpdated;
      this.wpfTextViewHost.Closed += OnTextViewHostClosed;

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
      Cleanup();
    }

    private void InitializeTextView() {
      this.textView = this.wpfTextViewHost.TextView;
      this.textView.BufferGraph.GraphBuffersChanged += OnGraphBuffersChanged;
      this.textView.BufferGraph.GraphBufferContentTypeChanged += OnGraphBufferContentTypeChanged;
      this.textView.Caret.PositionChanged += OnCaretPositionChanged;
      this.textView.TextViewModel.EditBuffer.PostChanged += OnBufferPostChanged;
    }

    private void Cleanup() {
      VsfSettings.SettingsUpdated -= OnSettingsUpdated;
      if ( this.wpfTextViewHost != null ) {
        this.wpfTextViewHost.Closed -= OnTextViewHostClosed;
        this.wpfTextViewHost = null;
      }
      if ( this.textView != null ) {
        this.textView.BufferGraph.GraphBuffersChanged -= OnGraphBuffersChanged;
        this.textView.BufferGraph.GraphBufferContentTypeChanged -= OnGraphBufferContentTypeChanged;
        this.textView.Caret.PositionChanged -= OnCaretPositionChanged;
        this.textView.TextViewModel.EditBuffer.PostChanged -= OnBufferPostChanged;
        this.textView = null;
      }
      if ( this.visual != null ) {
        this.visual.ViewBuffer -= OnViewBuffer;
        this.visual = null;
      }
      this.extensionRegistry = null;
    }

    private void OnTextViewHostClosed(object sender, EventArgs e) {
      Cleanup();
    }

    private void OnBufferPostChanged(object sender, EventArgs e) {
      // need to track buffer changes as well,
      // because text editing does not raise ITextCaret.PositionChanged
      UpdateCaretPosition(this.textView.Caret.Position);
    }

    private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      UpdateCaretPosition(e.NewPosition);
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

    private void UpdateCaretPosition(CaretPosition caret) {
      ITextBuffer currentBuffer = GetSelectedBuffer();
      SnapshotPoint? bufferPos = null;

      if ( currentBuffer == caret.BufferPosition.Snapshot.TextBuffer ) {
        bufferPos = caret.BufferPosition;
      } else {
        bufferPos = this.textView.BufferGraph.MapDownToBuffer(
           caret.BufferPosition, PointTrackingMode.Negative,
           currentBuffer, PositionAffinity.Predecessor
           );
        if ( !bufferPos.HasValue ) {
          bufferPos = this.textView.BufferGraph.MapUpToBuffer(
             caret.BufferPosition, PointTrackingMode.Negative,
             PositionAffinity.Predecessor, currentBuffer
             );
        }
      }
      if ( bufferPos.HasValue ) {
        this.model.BufferPosition = bufferPos.Value.Position.ToString();
      } else {
        this.model.BufferPosition = "--";
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
        String extension = extensionRegistry
          .GetExtensionsForContentType(b.ContentType)
          .FirstOrDefault();
        if ( String.IsNullOrEmpty(extension) ) {
          TextEditor.OpenBufferInPlainTextEditorAsReadOnly(b);
        } else {
          TextEditor.OpenBufferInEditorAsReadOnly(b, extension);
        }
      } catch ( Exception ex ) {
        MessageBox.Show(ex.Message, "Viasfora Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void RefreshBufferGraphList() {
      this.model.RefreshBuffers(this.textView.BufferGraph);
    }
  }
}
