using System;

namespace Winterdom.Viasfora {
  public class Telemetry {
    public bool Enabled { get; private set; }

    public Telemetry(bool enabled, EnvDTE80.DTE2 dte = null) {
    }

    public void WriteEvent(String eventName) {
    }

    public void WriteException(String msg, Exception ex) {
    }

    public void WriteTrace(String message) {
    }

    private void OnBeginShutdown() {
    }
  }
}
