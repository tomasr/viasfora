using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora {
  static class Telemetry {
    private static TelemetryClient client;
    public static bool Enabled { get; set; }

    public static void Initialize(EnvDTE80.DTE2 dte) {
      TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();

      client = new TelemetryClient(config);
      client.InstrumentationKey = "b59d19eb-668d-4ae3-b4c8-71536ebabbdc";

      client.Context.User.Id = GetUserId();
      client.Context.Session.Id = Guid.NewGuid().ToString();
      client.Context.Properties.Add("VsVersion", dte.Version);
      client.Context.Properties.Add("VsFullVersion", GetFullVsVersion());
      client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

      dte.Events.DTEEvents.OnBeginShutdown += OnBeginShutdown;

      var settings = SettingsContext.GetSettings();
      Enabled = settings.TelemetryEnabled;

      WriteEvent("Viasfora Started");
    }

    public static void WriteEvent(String eventName) {
#if !DEBUG
      if ( client != null && Enabled ) {
        client.TrackEvent(new EventTelemetry(eventName));
      }
#endif
    }

    public static void WriteException(String msg, Exception ex) {
#if !DEBUG
      if ( client != null && Enabled ) {
        ExceptionTelemetry telemetry = new ExceptionTelemetry(ex);
        telemetry.Properties.Add("Message", msg);
        client.TrackException(telemetry);
      }
#endif
    }

    private static void OnBeginShutdown() {
      client.Flush();
    }

    private static String GetFullVsVersion() {
      try {
        String baseDir = AppDomain.CurrentDomain.BaseDirectory;
        String devenv = Path.Combine(baseDir, "devenv.exe");
        var version = FileVersionInfo.GetVersionInfo(devenv);
        return version.ProductVersion;
      } catch {
        // Ignore if we cannot get
      }
      return "";
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
