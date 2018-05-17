using System;

namespace Winterdom.Viasfora.Contracts {
  public interface IVsfTelemetry {
    bool Enabled { get; }
    void WriteEvent(String eventName);
    void WriteException(String msg, Exception ex);
    void WriteTrace(String message);
    void FeatureStatus(String feature, bool enabled);
  }
}
