using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Tags;
using Microsoft.VisualStudio.Text;
using Winterdom.Viasfora.Util;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Classification;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IViewTaggerProvider))]
  [ContentType("text")]
  [TagType(typeof(ObfuscatedTextTag))]
  public class TextObfuscationProvider : IViewTaggerProvider {
    [Import]
    private IClassificationTypeRegistryService registryService = null;

    public ITagger<T> CreateTagger<T>(ITextView view, ITextBuffer buffer) where T : ITag {
      var obfuscationType = 
        registryService.GetClassificationType(Constants.OBFUSCATED_TEXT);
      return new TextObfuscationTagger(
                view, buffer, obfuscationType
              ) as ITagger<T>;
    }
  }


  public class TextObfuscationTagger : ITagger<ObfuscatedTextTag> {
    private ITextView theView;
    private ITextBuffer theBuffer;
    private IClassificationType obfuscationType;
    private bool enabled;
    private List<RegexEntry> expressionsToSearch;
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public TextObfuscationTagger(
          ITextView view, 
          ITextBuffer buffer, 
          IClassificationType obfuscationType) {
      this.theView = view;
      this.theBuffer = buffer;
      this.obfuscationType = obfuscationType;
      this.enabled = TextObfuscationState.Enabled;
      this.expressionsToSearch = 
        VsfSettings.TextObfuscationRegexes.ListFromJson<RegexEntry>();

      VsfSettings.SettingsUpdated += OnSettingsUpdated;
      TextObfuscationState.EnabledChanged += OnEnabledChanged;
      this.theView.Closed += OnViewClosed;
    }

    public IEnumerable<ITagSpan<ObfuscatedTextTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      bool scan = enabled
               && this.expressionsToSearch.Count > 0
               && spans.Count > 0;
      if ( !scan ) {
        yield break;
      }
      var tag = new ObfuscatedTextTag(this.obfuscationType);
      foreach ( var span in spans ) {
        ITextSnapshot snapshot = span.Snapshot;
        ITextSnapshotLine line = span.Start.GetContainingLine();
        do {
          var tags = expressionsToSearch
            .SelectMany(entry => entry.Match(line))
            .Select(match => new TagSpan<ObfuscatedTextTag>(match, tag));

          foreach ( var tagSpan in tags ) {
            yield return tagSpan;
          }
          if ( line.LineNumber + 1 >= snapshot.LineCount )
            break;
          line = snapshot.GetLineFromLineNumber(line.LineNumber + 1);
        } while ( line.End < span.End );
      }
    }

    private void OnViewClosed(object sender, EventArgs e) {
      if ( this.theView != null ) {
        this.theView.Closed -= OnViewClosed;
        VsfSettings.SettingsUpdated -= OnSettingsUpdated;
        TextObfuscationState.EnabledChanged -= OnEnabledChanged;
        this.theView = null;
        this.theBuffer = null;
        this.obfuscationType = null;
        this.expressionsToSearch = null;
      }
    }
    private void OnEnabledChanged(object sender, EventArgs e) {
      this.enabled = TextObfuscationState.Enabled;
      ITextSnapshot snapshot = this.theBuffer.CurrentSnapshot;
      SnapshotSpan span = new SnapshotSpan(snapshot, 0, snapshot.Length);
      ReportTagsChanged(span);
    }
    private void OnSettingsUpdated(object sender, EventArgs e) {
      this.expressionsToSearch = 
        VsfSettings.TextObfuscationRegexes.ListFromJson<RegexEntry>();
      ITextSnapshot snapshot = this.theBuffer.CurrentSnapshot;
      SnapshotSpan span = new SnapshotSpan(snapshot, 0, snapshot.Length);
      ReportTagsChanged(span);
    }

    private void ReportTagsChanged(SnapshotSpan span) {
      var handler = this.TagsChanged;
      if ( handler != null ) {
        handler(this, new SnapshotSpanEventArgs(span));
      }
    }
  }

  public static class TextObfuscationState {
    public static bool Enabled { get; private set; }
    public static event EventHandler EnabledChanged;

    public static void Invert() {
      Change(!Enabled);
    }
    public static void Change(bool enabled) {
      Enabled = enabled;
      var handler = EnabledChanged;
      if ( handler != null ) {
        handler(null, EventArgs.Empty);
      }
    }
  }
}
