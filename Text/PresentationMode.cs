using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class PresentationMode {
    private IWpfTextView theView;
    public PresentationMode(IWpfTextView textView) {
      this.theView = textView;
      VsfPackage.PresentationModeChanged += OnPresentationModeChanged;
      VsfSettings.SettingsUpdated += OnSettingsUpdated;
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

    void OnSettingsUpdated(object sender, EventArgs e) {
      SetZoomLevel(theView);
    }

    void OnTextViewClosed(object sender, EventArgs e) {
      if ( theView != null ) {
        VsfPackage.PresentationModeChanged -= OnPresentationModeChanged;
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        theView.Closed -= OnTextViewClosed;
        theView.ViewportWidthChanged -= OnViewportWidthChanged;
        theView = null;
      }
    }

    private void SetZoomLevel(IWpfTextView textView) {
      int zoomLevel = VsfPackage.GetPresentationModeZoomLevel();
      textView.ZoomLevel = zoomLevel;
    }
  }
}
