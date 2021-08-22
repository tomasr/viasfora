using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.LanguageService.Core.Utils;

namespace Winterdom.Viasfora.LanguageService.Core.RoslynTaggerProvider {
  public abstract class BaseRoslynTagger<TTag> : ITagger<TTag>
  where TTag : ITag {

    protected IVsfTelemetry Telemetry { get; }
    protected ITextBuffer Buffer { get; }
    protected ILanguageFactory LangFactory { get; }

    private TextBufferCodeAnalysesCache cache;

    protected BaseRoslynTagger(
      IVsfTelemetry telemetry,
      ITextBuffer buffer,
      ILanguageFactory langFactory
    ) {
      this.Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
      this.Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
      this.LangFactory = langFactory ?? throw new ArgumentNullException(nameof(langFactory));
    }

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public IEnumerable<ITagSpan<TTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      try {
        var tags = GetTagsInner(spans)
          .ToArray()
        ;
        return tags;
      } catch (Exception ex) {
        Telemetry.WriteException($@"Error analysing source code.
Spans: {spans.ToString()}
Source: {Buffer.CurrentSnapshot.GetText()}
", ex);
        return Enumerable.Empty<ITagSpan<TTag>>();
      }
    }
    private IEnumerable<ITagSpan<TTag>> GetTagsInner(NormalizedSnapshotSpanCollection spans) { 
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

      if (this.cache == null || this.cache.SyntaxRoot == null) {
        yield break;
      }

      var roslynTextSpan = spans[0].ToTextSpan();
      var rootNode = this.cache.SyntaxRoot.Value;

      if ( !rootNode.Span.Contains(roslynTextSpan) )
        yield break;

      var node = rootNode.FindNode(roslynTextSpan);

      // get roslyn tags and catch unhandled exceptions
      var tagsIter = GetTags(node, roslynTextSpan).GetEnumerator();
      var catchIter = new TryCatchEnumerator<(TextSpan Span, TTag Tag)>(tagsIter, ex => {
        if (ex is RoslynException) {
          this.Telemetry.WriteException($"Error getting tags from {this.GetType().FullName}", ex);
        } else {
          this.Telemetry.WriteException($"Error getting tags from {this.GetType().FullName}", RoslynException.Create(
            "Unknown error getting tags",
            node,
            roslynTextSpan,
            ex
          ));
        }
        return false;
      });

      while ( catchIter.MoveNext() ) {
        var snapshotSpan = catchIter.Current.Span.ToSnapshotSpan(snapshot);

        yield return new TagSpan<TTag>(snapshotSpan, catchIter.Current.Tag);
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
