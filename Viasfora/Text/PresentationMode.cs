using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public class PresentationMode {
    private IWpfTextView theView;
    private IVsfSettings settings;
    public PresentationMode(IWpfTextView textView, IVsfSettings settings) {
      this.theView = textView;
      this.settings = settings;

      VsfPackage.PresentationModeChanged += OnPresentationModeChanged;

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
        VsfPackage.PresentationModeChanged -= OnPresentationModeChanged;
        theView.Closed -= OnTextViewClosed;
        theView.ViewportWidthChanged -= OnViewportWidthChanged;
        theView = null;
      }
    }

    private void SetZoomLevel(IWpfTextView textView) {
      if ( VsfSettings.PresentationModeEnabled ) {
        int zoomLevel = VsfPackage.GetPresentationModeZoomLevel();
        textView.ZoomLevel = zoomLevel;
      }
    }
  }
}
