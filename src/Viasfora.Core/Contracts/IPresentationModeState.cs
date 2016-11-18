using System;

namespace Winterdom.Viasfora.Contracts {
  public interface IPresentationModeState {
    event EventHandler PresentationModeChanged;
    bool PresentationModeTurnedOn { get; }
    void TogglePresentationMode();
    void TurnOff(bool notifyChanges);
    int GetPresentationModeZoomLevel();
    T GetService<T>();
  }
}
