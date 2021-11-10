using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Winterdom.Viasfora.Xml;

namespace Winterdom.Viasfora.Options {
  [Guid(Guids.XmlOptions)]
  public class XmlOptionsPage : DialogPage {
    private ClassificationList colors;

    public override void SaveSettingsToStorage() {
      var settings = SettingsContext.GetService<IXmlSettings>();

      settings.XmlnsPrefixEnabled = XmlnsPrefixHighlightEnabled;
      settings.XmlCloseTagEnabled = XmlCloseTagHighlightEnabled;
      settings.XmlMatchTagsEnabled = XmlMatchTagsEnabled;

      this.colors.Save();
      settings.Save();
    }
    public override void LoadSettingsFromStorage() {
      var settings = SettingsContext.GetService<IXmlSettings>();
      XmlnsPrefixHighlightEnabled = settings.XmlnsPrefixEnabled;
      XmlCloseTagHighlightEnabled = settings.XmlCloseTagEnabled;
      XmlMatchTagsEnabled = settings.XmlMatchTagsEnabled;

      this.colors = new ClassificationList(new ColorStorage(this.Site));
      this.colors.Load(
        XmlConstants.XML_PREFIX,
        XmlConstants.XML_CLOSING,
        XmlConstants.XML_CLOSING_PREFIX,
        XmlConstants.RAZOR_CLOSING
        );
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

    [LocDisplayName("XML Namespace Prefix Color")]
    [Description("Foreground color used to highlight XML namespace prefixes")]
    [Category("XML Editor Colors")]
    public Color XmlnsPrefixForegroundColor {
      get { return this.colors.Get(XmlConstants.XML_PREFIX, true); }
      set { this.colors.Set(XmlConstants.XML_PREFIX, true, value); }
    }

    [LocDisplayName("XML Closing Tag Color")]
    [Description("Foreground color used to highlight XML closing element tags")]
    [Category("XML Editor Colors")]
    public Color XmlClosingTagForegroundColor {
      get { return this.colors.Get(XmlConstants.XML_CLOSING, true); }
      set { this.colors.Set(XmlConstants.XML_CLOSING, true, value); }
    }

    [LocDisplayName("XML Closing Prefix Color")]
    [Description("Foreground color used to highlight XML namespace prefixes in closing tags")]
    [Category("XML Editor Colors")]
    public Color XmlClosingPrefixForegroundColor {
      get { return this.colors.Get(XmlConstants.XML_CLOSING_PREFIX, true); }
      set { this.colors.Set(XmlConstants.XML_CLOSING_PREFIX, true, value); }
    }

    [LocDisplayName("Razor Closing Tag Color")]
    [Description("Foreground color used to highlight Razor closing tags")]
    [Category("XML Editor Colors")]
    public Color RazorClosingTagForegroundColor {
      get { return this.colors.Get(XmlConstants.RAZOR_CLOSING, true); }
      set { this.colors.Set(XmlConstants.RAZOR_CLOSING, true, value); }
    }
  }
}
