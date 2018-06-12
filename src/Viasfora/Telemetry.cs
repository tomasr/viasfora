using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Winterdom.Viasfora {
  public class Telemetry {
    private TelemetryClient client;
    public bool Enabled { get; private set; }

    public Telemetry(bool enabled, EnvDTE80.DTE2 dte = null) {
      TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();

      this.client = new TelemetryClient(config);
      this.client.InstrumentationKey = "b59d19eb-668d-4ae3-b4c8-71536ebabbdc";

      this.client.Context.User.Id = GetUserId();
      this.client.Context.Session.Id = Guid.NewGuid().ToString();
      this.client.Context.Properties.Add("Host", "VS");
      this.client.Context.Properties.Add("HostVersion", dte.Version);
      this.client.Context.Properties.Add("HostFullVersion", GetFullHostVersion());
      this.client.Context.Component.Version = GetViasforaVersion();

      if (enabled && dte != null) {
        dte.Events.DTEEvents.OnBeginShutdown += OnBeginShutdown;
      }

      Enabled = enabled;

      WriteEvent("Viasfora Started");
    }

    public void WriteEvent(String eventName) {
#if !DEBUG
      if ( this.client != null && Enabled ) {
        this.client.TrackEvent(new EventTelemetry(eventName));
      }
#endif
    }

    public void WriteEvent(EventTelemetry evt) {
#if !DEBUG
      if ( this.client != null && Enabled ) {
        this.client.TrackEvent(evt);
      }
#endif
    }

    public void WriteException(String msg, Exception ex) {
#if !DEBUG
      if ( this.client != null && Enabled ) {
        ExceptionTelemetry telemetry = new ExceptionTelemetry(ex);
        telemetry.Properties.Add("Message", msg);
        this.client.TrackException(telemetry);
      }
#endif
    }

    public void WriteTrace(String message) {
#if !DEBUG
      if ( this.client != null && Enabled ) {
        this.client.TrackTrace(message);
      }
#endif
    }

    private void OnBeginShutdown() {
      this.client.Flush();
    }

    private static String GetFullHostVersion() {
      try {
        String baseDir = AppDomain.CurrentDomain.BaseDirectory;
        String devenv = Path.Combine(baseDir, "msenv.dll");
        var version = FileVersionInfo.GetVersionInfo(devenv);
        return version.ProductVersion;
      } catch {
        // Ignore if we cannot get
      }
      return "";
    }

    private static String GetViasforaVersion() {
      var assembly = typeof(Telemetry).Assembly;
      var fileVersion = assembly
                       .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                       .Cast<AssemblyFileVersionAttribute>()
                       .First().Version;
      return fileVersion;
    }

    private static String GetUserId() {
      var user = Environment.MachineName + "\\" + Environment.UserName;
      var bytes = Encoding.UTF8.GetBytes(user);
      using ( var sha = SHA256.Create() ) {
        return Convert.ToBase64String(sha.ComputeHash(bytes));
      }
    }
  }
}
