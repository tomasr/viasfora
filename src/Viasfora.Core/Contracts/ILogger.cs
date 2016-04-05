using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Contracts {
  public interface ILogger {
    void LogInfo(String format, params object[] args);
    void LogError(String message, Exception ex);
  }
}
