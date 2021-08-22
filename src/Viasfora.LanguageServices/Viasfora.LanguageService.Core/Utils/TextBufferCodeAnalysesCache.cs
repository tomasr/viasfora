using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using System;
using System.Threading.Tasks;

namespace Winterdom.Viasfora.LanguageService.Core.Utils {

  internal class TextBufferCodeAnalysesCache {
    public Workspace Workspace { get; private set; }
    public Document Document { get; private set; }
    public Lazy<SemanticModel> SemanticModel { get; }
    public Lazy<SyntaxNode> SyntaxRoot { get; }
    public ITextSnapshot Snapshot { get; private set; }

    private TextBufferCodeAnalysesCache() {
      this.SemanticModel = new Lazy<SemanticModel>(() => this.Document.GetSemanticModelAsync().ConfigureAwait(false).GetAwaiter().GetResult());
      this.SyntaxRoot = new Lazy<SyntaxNode>(() => Document.GetSyntaxRootAsync().ConfigureAwait(false).GetAwaiter().GetResult());
    }

    public static async Task<TextBufferCodeAnalysesCache> ResolveAsync(ITextBuffer buffer, ITextSnapshot snapshot) {
      var workspace = buffer.GetWorkspace();
      var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
      if ( document == null ) {
        // Razor cshtml returns a null document for some reason.
        return null;
      }

      // the ConfigureAwait() calls are important,
      // otherwise we'll deadlock VS
      var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
      var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
      return new TextBufferCodeAnalysesCache {
        Workspace = workspace,
        Document = document,
        Snapshot = snapshot
      };
    }
  }
}
