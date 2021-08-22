using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.LanguageService.Core.ArgumentValidationTagger;

namespace Winterdom.Viasfora.LanguageService.CSharp.ArgumentValidationTagger {

  [Export(typeof(IViewTaggerProvider))]
  [ContentType(ContentTypes.CSharp)]
  [TagType(typeof(ArgumentValidationTag))]
  public sealed class ArgumentValidationTaggerProvider : BaseArgumentValidationTaggerProvider {

    [ImportingConstructor]
    public ArgumentValidationTaggerProvider(
      IVsfTelemetry telemetry,
      IClassificationTypeRegistryService classificationRegistry,
      ILanguageFactory languageFactory,
      IVsfSettings settings
    ) : base(telemetry, classificationRegistry, languageFactory, settings) {
    }

    public override TextSpan IsArgumentValidationSpan(SyntaxNode node) {
      if ( !SyntaxHelper.IsAnyIfArgumentThrowSyntaxStatement(node) )
        return new TextSpan();

      return node.FullSpan;
    }
  }
}
