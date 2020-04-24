using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.LanguageService.Core.RoslynTaggerProvider;

namespace Winterdom.Viasfora.LanguageService.Core.ArgumentValidationTagger {
  public class ArgumentValidationTagger : BaseRoslynTagger<ArgumentValidationTag> {
    private BaseArgumentValidationTaggerProvider provider;
    private ArgumentValidationTag argumentValidationClassification;

    internal ArgumentValidationTagger(
      IVsfTelemetry telemetry, 
      ITextBuffer buffer, 
      BaseArgumentValidationTaggerProvider provider
    ) : base (telemetry, buffer, provider.LanguageFactory) {
      this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

      this.argumentValidationClassification = provider.GetTag(Constants.LANGUAGESERVICE_ARGUMENT_VALIDATION_NAME);
    }

    protected override IEnumerable<(TextSpan Span, ArgumentValidationTag Tag)> GetTags(SyntaxNode node, TextSpan span) {
      var resultSpan = this.provider.IsArgumentValidationSpan(node);
      if ( span.IsEmpty )
        yield break;
      else
        yield return (Span: resultSpan, Tag: this.argumentValidationClassification);
    }

    protected override bool IsEnabled(ILanguage lang) => lang.Settings.ReduceOpacityForArgumentValidation && this.provider.Settings.ArgumentValidationClassifierEnabled;
  }
}
