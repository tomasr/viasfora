using System;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Languages {
  public class DefaultLanguage : LanguageInfo {
    static readonly String[] empty = new String[0];
    protected override String[] ControlFlowDefaults {
      get { return empty; }
    }
    protected override String[] LinqDefaults {
      get { return empty; }
    }
    protected override String[] VisibilityDefaults {
      get { return empty; }
    }

    protected override string[] SupportedContentTypes {
      get { return empty; }
    }
    public override string KeyName {
      get { return "Text"; }
    }

    [ImportingConstructor]
    public DefaultLanguage(IVsfSettings settings) : base(settings) {
    }


    public override bool MatchesContentType(IContentType contentType) {
      return true;
    }
    protected override IBraceScanner NewBraceScanner() {
      return new DefaultBraceScanner();
    }

    private class NoFirstLineCommentParser : IFirstLineCommentParser {
      public string Parse(ITextChars tc) {
        return "";
      }
    }
  }
}
