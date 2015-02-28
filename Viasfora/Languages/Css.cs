using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class Css : LanguageInfo {
    public const String ContentType = "css";
    public const String SassContentType = "SCSS";
    public const String LessContentType = "LESS";

    public override String KeyName {
      get { return Constants.Css; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType, SassContentType, LessContentType }; }
    }
    protected override String[] ControlFlowDefaults {
      get { return EMPTY; }
    }
    protected override String[] LinqDefaults {
      get { return EMPTY; }
    }
    protected override String[] VisibilityDefaults {
      get { return EMPTY; }
    }

    [ImportingConstructor]
    public Css(IVsfSettings settings) : base(settings) {
    }

    public override IEscapeSequenceParser NewEscapeSequenceParser(string text) {
      return new CssEscapeSequenceParser(text);
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new CssBraceExtractor();
    }
  }
}
