using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora {
  [Export(typeof(IVsfTelemetry))]
  public class TelemetryService : IVsfTelemetry {
    private readonly Telemetry telemetry;

    public bool Enabled  => this.telemetry.Enabled;

    [ImportingConstructor]
    public TelemetryService(SVsServiceProvider serviceProvider, ITypedSettingsStore settings) {
      // We can't ask for IVsfSettings here because we'd create a circular
      // dependency chain, which would cause MEF to fail.
      bool telemetryEnabled = false;//settings.GetBoolean(nameof(IVsfSettings.TelemetryEnabled), true);
      var dte = (EnvDTE80.DTE2)serviceProvider.GetService(typeof(SDTE));
      this.telemetry = new Telemetry(telemetryEnabled, dte);
    }

    public void WriteEvent(string eventName) {
      this.telemetry.WriteEvent(eventName);
    }

    public void WriteException(string msg, Exception ex) {
      this.telemetry.WriteException(msg, ex);
    }

    public void WriteTrace(string message) {
      this.telemetry.WriteTrace(message);
    }

    public void FeatureStatus(String feature, bool enabled) {
    }
  }
}
