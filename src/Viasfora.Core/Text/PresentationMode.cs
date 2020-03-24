using Microsoft.VisualStudio.Text.Editor;
using System;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {
  public class PresentationMode {
    private IWpfTextView theView;
    private IVsfSettings settings;
    private IPresentationModeState state;
    public PresentationMode(IWpfTextView textView, IPresentationModeState state, IVsfSettings settings) {
      this.theView = textView;
      this.settings = settings;
      this.state = state;

      state.PresentationModeChanged += OnPresentationModeChanged;

      settings.SettingsChanged += OnSettingsChanged;
      textView.Closed += OnTextViewClosed;
      textView.ViewportWidthChanged += OnViewportWidthChanged;
    }

    private void OnPresentationModeChanged(object sender, EventArgs e) {
      if ( this.theView != null ) {
        SetZoomLevel(this.theView);
      }
    }

    void OnViewportWidthChanged(object sender, EventArgs e) {
      this.theView.ViewportWidthChanged -= OnViewportWidthChanged;
      SetZoomLevel(this.theView);
    }

    void OnSettingsChanged(object sender, EventArgs e) {
      SetZoomLevel(this.theView);
    }

    void OnTextViewClosed(object sender, EventArgs e) {
      if ( this.settings != null ) {
        this.settings.SettingsChanged -= OnSettingsChanged;
        this.settings = null;
      }
      if ( this.theView != null ) {
        this.state.PresentationModeChanged -= OnPresentationModeChanged;
        this.theView.Closed -= OnTextViewClosed;
        this.theView.ViewportWidthChanged -= OnViewportWidthChanged;
        this.theView = null;
      }
    }

    private void SetZoomLevel(IWpfTextView textView) {
      // don't try overriding the zoom level for
      // a peek definition window
      if ( textView.IsPeekTextWindow() )
        return;
      if ( this.settings.PresentationModeEnabled  ) {
        var textViewState = textView.Get<PresentationModeViewState>();
        bool pmEnabled() { return textViewState?.Enabled ?? false; }

        int zoomLevel = this.state.GetPresentationModeZoomLevel();

        if (this.state.PresentationModeTurnedOn) {
          if (!pmEnabled()) {
            textView.ZoomLevel = zoomLevel;
            textView.Set(new PresentationModeViewState(true));
          }
        } else {
          if ( pmEnabled() ) {
            textViewState.Enabled = false;
            textView.ZoomLevel = zoomLevel;
          }
        }
      }
    }
  }

  internal class PresentationModeViewState {
    public bool Enabled { get; set; }

    public PresentationModeViewState(bool state) {
      this.Enabled = state;
    }
  }
}
