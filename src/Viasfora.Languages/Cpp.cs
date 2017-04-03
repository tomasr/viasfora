using System;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages.Sequences;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Languages {
  [Export(typeof(ILanguage))]
  class Cpp : CBasedLanguage {
    private readonly static String[] knownTypes =
      new String[] { "C/C++", "HLSL" };
    static readonly String[] CPP_KEYWORDS = {
         "if", "else", "while", "do", "for", "each", "switch",
         "break", "continue", "return", "goto", "throw"
      };
    static readonly String[] CPP_VIS_KEYWORDS = {
         "public", "private", "protected", "internal", "friend"
      };
    protected override String[] ControlFlowDefaults => CPP_KEYWORDS;
    protected override String[] LinqDefaults => EMPTY;
    protected override String[] VisibilityDefaults => CPP_VIS_KEYWORDS;
    public override String KeyName => Constants.Cpp;

    protected override String[] SupportedContentTypes
      => knownTypes;

    [ImportingConstructor]
    public Cpp(IVsfSettings settings) : base(settings) {
    }

    public override IStringScanner NewStringScanner(string text)
      => new CStringScanner(text);
  }
}
