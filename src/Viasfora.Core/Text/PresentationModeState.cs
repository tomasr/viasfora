using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IPresentationModeState))]
  public class PresentationModeState : IPresentationModeState {
    public bool PresentationModeTurnedOn { get; private set; }
    private PresentationModeFontChanger fontChanger;
    private IVsfSettings settings;
    private IVsfTelemetry telemetry;

    public event EventHandler PresentationModeChanged;

    [ImportingConstructor]
    public PresentationModeState(IVsfSettings settingsManager, IVsfTelemetry telemetry) {
      this.fontChanger = new PresentationModeFontChanger(this);
      this.settings = settingsManager;
      this.telemetry = telemetry;
    }

    public int GetPresentationModeZoomLevel() {
      return PresentationModeTurnedOn
        ? settings.PresentationModeEnabledZoom
        : settings.PresentationModeDefaultZoom;
    }

    public void TogglePresentationMode() {
      PresentationModeTurnedOn = !PresentationModeTurnedOn;
      if (PresentationModeChanged != null) {
        PresentationModeChanged(this, EventArgs.Empty);
      }
      if (PresentationModeTurnedOn) {
        fontChanger.TurnOn();
        telemetry.WriteEvent("Presentation Mode");
      } else {
        fontChanger.TurnOff();
      }
    }

    public void TurnOff(bool notifyChanges) {
      fontChanger.TurnOff(notifyChanges);
    }

    public T GetService<T>() {
      return (T)ServiceProvider.GlobalProvider.GetService(typeof(T));
    }

  }
}
