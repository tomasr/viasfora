using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using Winterdom.Viasfora.Languages;
using Microsoft.CodeAnalysis.Text;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.LanguageService.Core.ArgumentValidationTagger {


  public abstract class BaseArgumentValidationTaggerProvider: IViewTaggerProvider 
  {

    public IVsfTelemetry Telemetry { get; }
    public IClassificationTypeRegistryService ClassificationRegistry { get; }
    public ILanguageFactory LanguageFactory { get; }
    public IVsfSettings Settings { get; }


    protected BaseArgumentValidationTaggerProvider(
      IVsfTelemetry telemetry,
      IClassificationTypeRegistryService classificationRegistry,
      ILanguageFactory languageFactory,
      IVsfSettings settings
    ) {
      this.Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
      this.ClassificationRegistry = classificationRegistry ?? throw new ArgumentNullException(nameof(classificationRegistry));
      this.LanguageFactory = languageFactory ?? throw new ArgumentNullException(nameof(languageFactory));
      this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
      return new ArgumentValidationTagger(this.Telemetry, buffer, this) as ITagger<T>;
    }

    public ArgumentValidationTag GetTag(string name) {
      var type = ClassificationRegistry.GetClassificationType(name);
      return new ArgumentValidationTag(type);
    }

    public abstract TextSpan IsArgumentValidationSpan(SyntaxNode node);
  }
}
