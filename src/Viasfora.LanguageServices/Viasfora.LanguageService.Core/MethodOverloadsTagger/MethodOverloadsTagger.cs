using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.LanguageService.Core.RoslynTaggerProvider;

namespace Winterdom.Viasfora.LanguageService.Core.MethodOverloadsTagger {
  public class MethodOverloadsTagger : BaseRoslynTagger<MethodOverloadsTag> {
    private BaseMethodOverloadsTaggerProvider provider;
    private MethodOverloadsTag methodOverloadsClassification;

    internal MethodOverloadsTagger(
      IVsfTelemetry telemetry, 
      ITextBuffer buffer, 
      BaseMethodOverloadsTaggerProvider provider
    ) : base(telemetry, buffer, provider.LanguageFactory) {
      this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

      this.methodOverloadsClassification = provider.GetTag(Constants.LANGAUGESERVICE_METHOD_OVERLOAD_NAME);
    }

    protected override IEnumerable<(TextSpan Span, MethodOverloadsTag Tag)> GetTags(SyntaxNode node, TextSpan span) {
      var resultSpan = this.provider.IsMethodOverload(node);
      if ( span.IsEmpty )
        yield break;
      else
        yield return (Span: resultSpan, Tag: this.methodOverloadsClassification);
    }

    protected override bool IsEnabled(ILanguage lang) => lang.Settings.ReduceOpacityForMethodOverloads && this.provider.Settings.MethodOverloadsClassifierEnabled;
  }
}
