using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Languages.BraceExtractors;
using Winterdom.Viasfora.Languages.CommentParsers;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  class FSharp : LanguageInfo {
    public const String ContentType = "F#";
    static readonly String[] KEYWORDS = {
         "if", "then", "elif", "else", "match", "with",
         "for", "do", "to", "done", "while", "rec",
         "failwith", "yield"
      };
    static readonly String[] LINQ_KEYWORDS = {
         "query", "select", "seq"
      };
    static readonly String[] VIS_KEYWORDS = {
         "public", "private", "internal"
      };
    protected override String[] ControlFlowDefaults {
      get { return KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return VIS_KEYWORDS; }
    }
    protected override String KeyName {
      get { return "FSharp"; }
    }
    protected override String[] ContentTypes {
      get { return new String[] { ContentType }; }
    }
    public override string BraceList {
      get { return "(){}[]"; }
    }
    public override bool SupportsEscapeSeqs {
      get { return true; }
    }
    public override IBraceExtractor NewBraceExtractor() {
      return new FSharpBraceExtractor(this);
    }
    public override IFirstLineCommentParser NewFirstLineCommentParser() {
      return new FSharpFirstLineCommentParser();
    }
  }
}
