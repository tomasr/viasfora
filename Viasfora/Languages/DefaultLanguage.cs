using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

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
    public override bool MatchesContentType(IContentType contentType) {
      return true;
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new DefaultBraceExtractor();
    }

    private class NoFirstLineCommentParser : IFirstLineCommentParser {
      public string Parse(ITextChars tc) {
        return "";
      }
    }
  }
}
