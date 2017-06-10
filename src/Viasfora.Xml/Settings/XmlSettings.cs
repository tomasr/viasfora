using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Xml.Settings {
  [Export(typeof(IXmlSettings))]
  public class XmlSettings : SettingsBase, IXmlSettings {
    public event EventHandler SettingsChanged;

    public bool XmlnsPrefixEnabled {
      get { return GetBoolean(nameof(XmlnsPrefixEnabled), true); }
      set { SetValue(nameof(XmlnsPrefixEnabled), value); }
    }
    public bool XmlCloseTagEnabled {
      get { return GetBoolean(nameof(XmlCloseTagEnabled), true); }
      set { SetValue(nameof(XmlCloseTagEnabled), value); }
    }
    public bool XmlMatchTagsEnabled {
      get { return GetBoolean(nameof(XmlMatchTagsEnabled), true); }
      set { SetValue(nameof(XmlMatchTagsEnabled), value); }
    }

    [ImportingConstructor]
    public XmlSettings(ISettingsStore store, IStorageConversions converter) : base(store, converter) {
    }

    public void Load() {
      this.Store.Load();
    }

    public void Save() {
      this.Store.Save();
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
