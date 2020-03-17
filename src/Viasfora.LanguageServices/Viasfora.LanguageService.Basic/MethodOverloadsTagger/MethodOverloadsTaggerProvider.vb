Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities
Imports System.ComponentModel.Composition
Imports Winterdom.Viasfora.Languages
Imports Winterdom.Viasfora.LanguageService.Core.MethodOverloadsTagger

Namespace MethodOverloadsTagger

  <Export(GetType(IViewTaggerProvider))>
  <ContentType(ContentTypes.VB)>
  <TagType(GetType(MethodOverloadsTag))>
  Public Class MethodOverloadsTaggerProvider
    Inherits BaseMethodOverloadsTaggerProvider

    <ImportingConstructor>
    Public Sub New(classificationRegistry As IClassificationTypeRegistryService, languageFactory As ILanguageFactory, settings As IVsfSettings)
      MyBase.New(classificationRegistry, languageFactory, settings)
    End Sub

    Public Overrides Function IsMethodOverload(node As SyntaxNode) As TextSpan
      If Not SyntaxHelper.IsMethodOverload(node) Then
        Return New TextSpan()
      End If

      Return node.FullSpan
    End Function
  End Class

End Namespace