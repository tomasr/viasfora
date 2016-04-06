using System;

namespace Winterdom.Viasfora.Contracts {
  public interface IPresentationModeState {
    event EventHandler PresentationModeChanged;
    bool PresentationModeTurnedOn { get; }
    void TogglePresentationMode();
    int GetPresentationModeZoomLevel();
    T GetService<T>();
  }
}
