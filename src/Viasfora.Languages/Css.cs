using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class Css : LanguageInfo {
    public const String ContentType = "css";
    public const String SassContentType = "SCSS";
    public const String LessContentType = "LESS";

    public override String KeyName => Constants.Css;

    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, SassContentType, LessContentType }; }
    }
    protected override String[] ControlFlowDefaults => EMPTY;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => EMPTY;

    [ImportingConstructor]
    public Css(IVsfSettings settings) : base(settings) {
    }

    public override IStringScanner NewStringScanner(string text)
      => new CssStringScanner(text);
    protected override IBraceScanner NewBraceScanner()
      => new CssBraceScanner();
  }
}
