using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora {
  [Export(typeof(IVsfTelemetry))]
  public class TelemetryService : IVsfTelemetry {
    public bool Enabled {
      get { return Telemetry.Enabled; }
    }

    public void WriteEvent(string eventName) {
      Telemetry.WriteEvent(eventName);
    }

    public void WriteException(string msg, Exception ex) {
      Telemetry.WriteException(msg, ex);
    }

    public void WriteTrace(string message) {
      Telemetry.WriteTrace(message);
    }
  }
}
