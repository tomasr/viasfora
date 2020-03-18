Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities
Imports System.ComponentModel.Composition
Imports Winterdom.Viasfora.Contracts
Imports Winterdom.Viasfora.Languages
Imports Winterdom.Viasfora.LanguageService.Core.ArgumentValidationTagger

Namespace ArgumentValidationTagger

  <Export(GetType(IViewTaggerProvider))>
  <ContentType(ContentTypes.VB)>
  <TagType(GetType(ArgumentValidationTag))>
  Public Class ArgumentValidationTaggerProvider
    Inherits BaseArgumentValidationTaggerProvider

    <ImportingConstructor>
    Public Sub New(
      telemetry As IVsfTelemetry,
      classificationRegistry As IClassificationTypeRegistryService,
      languageFactory As ILanguageFactory,
      settings As IVsfSettings
    )
      MyBase.New(telemetry, classificationRegistry, languageFactory, settings)
    End Sub

    Public Overrides Function IsArgumentValidationSpan(node As SyntaxNode) As TextSpan
      If Not SyntaxHelper.IsAnyIfArgumentThrowSyntaxStatement(node) Then
        Return New TextSpan()
      End If

      Return node.FullSpan
    End Function
  End Class

End Namespace