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
      if ( theView != null ) {
        SetZoomLevel(theView);
      }
    }

    void OnViewportWidthChanged(object sender, EventArgs e) {
      theView.ViewportWidthChanged -= OnViewportWidthChanged;
      SetZoomLevel(theView);
    }

    void OnSettingsChanged(object sender, EventArgs e) {
      SetZoomLevel(theView);
    }

    void OnTextViewClosed(object sender, EventArgs e) {
      if ( this.settings != null ) {
        this.settings.SettingsChanged -= OnSettingsChanged;
        this.settings = null;
      }
      if ( theView != null ) {
        state.PresentationModeChanged -= OnPresentationModeChanged;
        theView.Closed -= OnTextViewClosed;
        theView.ViewportWidthChanged -= OnViewportWidthChanged;
        theView = null;
      }
    }

    private void SetZoomLevel(IWpfTextView textView) {
      // don't try overriding the zoom level for
      // a peek definition window
      if ( textView.IsPeekTextWindow() )
        return;
      if ( this.settings.PresentationModeEnabled  ) {
        int zoomLevel = state.GetPresentationModeZoomLevel();
        // VS2015 supports automatic sync of all text windows with the 
        // zoom level once you zoom one
        // so if the current zoomLevel is not 100%, just ignore it.
        if ( textView.ZoomLevel != 100 && state.PresentationModeTurnedOn )
          return;
        textView.ZoomLevel = zoomLevel;
      }
    }
  }
}
