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
        ? this.settings.PresentationModeEnabledZoom
        : this.settings.PresentationModeDefaultZoom;
    }

    public void TogglePresentationMode() {
      PresentationModeTurnedOn = !PresentationModeTurnedOn;
      PresentationModeChanged?.Invoke(this, EventArgs.Empty);
      if (PresentationModeTurnedOn) {
        this.fontChanger.TurnOn();
        this.telemetry.WriteEvent("Presentation Mode");
      } else {
        this.fontChanger.TurnOff();
      }
    }

    public void TurnOff(bool notifyChanges) {
      this.fontChanger.TurnOff(notifyChanges);
    }

    public T GetService<T>() {
      return (T)ServiceProvider.GlobalProvider.GetService(typeof(T));
    }

  }
}
