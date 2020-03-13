using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.LanguageService.Core.Utils;

namespace Winterdom.Viasfora.LanguageService.Core.RoslynTaggerProvider {
  public abstract class BaseRoslynTagger<TTag> : ITagger<TTag>
  where TTag : ITag {

    protected ITextBuffer Buffer { get; }
    protected ILanguageFactory LangFactory { get; }

    private TextBufferCodeAnalysesCache cache;

    protected BaseRoslynTagger(
      ITextBuffer buffer,
      ILanguageFactory langFactory
    ) {
      this.Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
      this.LangFactory = langFactory ?? throw new ArgumentNullException(nameof(langFactory));
    }

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public IEnumerable<ITagSpan<TTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 )
        yield break;

      ILanguage lang = GetLanguageByContentType(this.Buffer.ContentType);
      if ( !lang.Settings.Enabled || !this.IsEnabled(lang) ) {
        yield break;
      }

      var snapshot = spans[0].Snapshot;

      if ( this.cache == null || this.cache.Snapshot != snapshot ) {
        this.cache = TextBufferCodeAnalysesCache.ResolveAsync(this.Buffer, snapshot)
          .ConfigureAwait(false).GetAwaiter().GetResult();
      }

      var roslynTextSpan = spans[0].ToTextSpan();
      var rootNode = this.cache.SyntaxRoot.Value;

      if ( !rootNode.Span.Contains(roslynTextSpan) )
        yield break;

      var node = rootNode.FindNode(roslynTextSpan);
      var roslynTags = GetTags(node, roslynTextSpan);

      foreach (var item in roslynTags ) {
        var snapshotSpan = item.Span.ToSnapshotSpan(snapshot);

        yield return new TagSpan<TTag>(snapshotSpan, item.Tag);
      }
    }

    private ILanguage GetLanguageByContentType(IContentType contentType) {
      return this.LangFactory.TryCreateLanguage(contentType);
    }

    void OnSettingsChanged(object sender, EventArgs e) {
      if ( this.Buffer == null )
        return;
      TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(this.Buffer.CurrentSnapshot.GetSpan()));
    }

    protected virtual bool IsEnabled(ILanguage lang) => true;
    protected abstract IEnumerable<(TextSpan Span, TTag Tag)> GetTags(SyntaxNode node, TextSpan span);
  }
}
