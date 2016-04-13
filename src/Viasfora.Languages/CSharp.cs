using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.BraceScanners;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Rainbow;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  public class CSharp : CBasedLanguage {
    public const String ContentType = "CSharp";
    static readonly String[] CS_KEYWORDS = {
         "if", "else", "while", "do", "for", "foreach", 
         "switch", "break", "continue", "return", "goto", "throw",
         "yield"
      };
    static readonly String[] CS_LINQ_KEYWORDS = {
         "select", "let", "where", "join", "orderby", "group",
         "by", "on", "equals", "into", "from", "descending",
         "ascending"
      };
    static readonly String[] CS_VIS_KEYWORDS = {
         "public", "private", "protected", "internal"
      };
    protected override String[] ControlFlowDefaults {
      get { return CS_KEYWORDS; }
    }
    protected override String[] LinqDefaults {
      get { return CS_LINQ_KEYWORDS; }
    }
    protected override String[] VisibilityDefaults {
      get { return CS_VIS_KEYWORDS; }
    }
    public override String KeyName {
      get { return Constants.CSharp; }
    }
    protected override String[] SupportedContentTypes {
      get { return new String[] { ContentType }; }
    }

    protected override IBraceScanner NewBraceScanner() {
      return new CSharpBraceScanner();
    }
    public override IStringScanner NewStringScanner(string text) {
      return new CSharpStringScanner(text);
    }

    [ImportingConstructor]
    public CSharp(IVsfSettings settings) : base(settings) {
    }
  }
}
