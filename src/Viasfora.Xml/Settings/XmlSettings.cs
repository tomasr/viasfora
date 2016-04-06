using System;
using System.ComponentModel.Composition;

namespace Winterdom.Viasfora.Xml.Settings {
  [Export(typeof(IXmlSettings))]
  public class XmlSettings : IXmlSettings {
    private IVsfSettings settings;

    public event EventHandler SettingsChanged {
      add { settings.SettingsChanged += value; }
      remove { settings.SettingsChanged -= value; }
    }

    public bool XmlnsPrefixEnabled {
      get { return settings.GetBoolean(nameof(XmlnsPrefixEnabled), true); }
      set { settings.SetValue(nameof(XmlnsPrefixEnabled), value); }
    }
    public bool XmlCloseTagEnabled {
      get { return settings.GetBoolean(nameof(XmlCloseTagEnabled), true); }
      set { settings.SetValue(nameof(XmlCloseTagEnabled), value); }
    }
    public bool XmlMatchTagsEnabled {
      get { return settings.GetBoolean(nameof(XmlMatchTagsEnabled), true); }
      set { settings.SetValue(nameof(XmlMatchTagsEnabled), value); }
    }

    [ImportingConstructor]
    public XmlSettings(IVsfSettings settings) {
      this.settings = settings;
    }

    public void Save() {
      this.settings.Save();
    }
  }
}
