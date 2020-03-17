using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.LanguageService.Core.MethodOverloadsTagger;

namespace Winterdom.Viasfora.LanguageService.CSharp.MethodOverloadsTagger {
  [Export(typeof(IViewTaggerProvider))]
  [ContentType(ContentTypes.CSharp)]
  [TagType(typeof(MethodOverloadsTag))]
  public sealed class MethodOverloadsTaggerProvider : BaseMethodOverloadsTaggerProvider {

    [ImportingConstructor]
    public MethodOverloadsTaggerProvider(
      IClassificationTypeRegistryService classificationRegistry,
      ILanguageFactory languageFactory,
      IVsfSettings settings
    ) : base(classificationRegistry, languageFactory, settings) {
    }

    public override TextSpan IsMethodOverload(SyntaxNode node) {
      if ( !SyntaxHelper.IsMethodOverload(node) )
        return new TextSpan();

      return node.FullSpan;
    }
  }
}
