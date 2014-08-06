using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Rainbow;

namespace Winterdom.Viasfora {
  public interface IVsfSettings {
    bool KeywordClassifierEnabled { get; set; }
    bool EscapeSeqHighlightEnabled { get; set; }
    
    bool XmlnsPrefixHighlightEnabled { get; set; }
    bool XmlCloseTagHighlightEnabled { get; set; }
    bool XmlMatchTagsEnabled { get; set; }

    bool CurrentLineHighlightEnabled { get; set; }
    bool CurrentColumnHighlightEnabled { get; set; }
    double HighlightLineWidth { get; set; }

    bool RainbowTagsEnabled { get; set; }
    long RainbowCtrlTimer { get; set; }
    RainbowHighlightMode RainbowHighlightMode { get; set; }
    bool RainbowToolTipsEnabled { get; set; }

    bool PresentationModeEnabled { get; set; }
    int PresentationModeDefaultZoomLevel { get; set; }
    int PresentationModeEnabledZoomLevel { get; set; }

    bool ModelinesEnabled { get; set; }
    int ModelinesNumLines { get; set; }

    bool DevMarginEnabled { get; set; }
    bool TextCompletionEnabled { get; set; }
    bool TCCompleteDuringTyping { get; set; }
    bool TCHandleCompleteWord { get; set; }
    Outlining.AutoExpandMode AutoExpandRegions { get; set; }
    bool BoldAsItalicsEnabled { get; set; }
    String TextObfuscationRegexes { get; set; }

    event EventHandler SettingsChanged;
    void Load();
    void Save();
  }
}
