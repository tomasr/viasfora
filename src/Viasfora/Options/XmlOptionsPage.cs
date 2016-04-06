using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Winterdom.Viasfora.Xml;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.XmlOptions)]
  public class XmlOptionsPage : DialogPage {
    public override void SaveSettingsToStorage() {
      base.SaveSettingsToStorage();
      var settings = SettingsContext.GetSpecificSettings<IXmlSettings>();

      settings.XmlnsPrefixEnabled = XmlnsPrefixHighlightEnabled;
      settings.XmlCloseTagEnabled = XmlCloseTagHighlightEnabled;
      settings.XmlMatchTagsEnabled = XmlMatchTagsEnabled;
    }
    public override void LoadSettingsFromStorage() {
      base.LoadSettingsFromStorage();
      var settings = SettingsContext.GetSpecificSettings<IXmlSettings>();
      XmlnsPrefixHighlightEnabled = settings.XmlnsPrefixEnabled;
      XmlCloseTagHighlightEnabled = settings.XmlCloseTagEnabled;
      XmlMatchTagsEnabled = settings.XmlMatchTagsEnabled;
    }

    // XML Editor Extensions
    [LocDisplayName("Highlight XML Namespace Prefix")]
    [Description("Enables highlighting XML Namespace prefixes")]
    [Category("XML Editor")]
    public bool XmlnsPrefixHighlightEnabled { get; set; }

    [LocDisplayName("Highlight XML Closing Tags")]
    [Description("Enables highlighting XML closing element tags")]
    [Category("XML Editor")]
    public bool XmlCloseTagHighlightEnabled { get; set; }

    [LocDisplayName("Match Element Tags")]
    [Description("Enables highlighting XML element opening/closing tags")]
    [Category("XML Editor")]
    public bool XmlMatchTagsEnabled { get; set; }

  }
}
