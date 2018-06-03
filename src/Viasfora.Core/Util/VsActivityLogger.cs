using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Util {
  [Export(typeof(ILogger))]
  public class VsActivityLogger : ILogger {
    private readonly IVsActivityLog activityLog;
    private readonly IVsfTelemetry telemetry;

    [ImportingConstructor]
    public VsActivityLogger(SVsServiceProvider serviceProvider, IVsfTelemetry telemetry) {
      this.telemetry = telemetry;
      this.activityLog = serviceProvider.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
    }

    public void LogError(string message, Exception ex) {
      var log = this.activityLog;
      if ( log != null ) {
        log.LogEntry(
          (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
          "Viasfora",
          String.Format("{0}. Exception: {1}", message, ex)
        );
      }
      this.telemetry.WriteException(message, ex);
    }

    public void LogInfo(string format, params object[] args) {
      var log = this.activityLog;
      if ( log != null ) {
        log.LogEntry(
          (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
          "Viasfora",
          String.Format(format, args)
        );
      }
    }
  }
}
