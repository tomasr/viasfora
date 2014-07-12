using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Languages;
using Winterdom.Viasfora.Tags;

namespace Winterdom.Viasfora.Text {
  [Export(typeof(IIntellisenseControllerProvider))]
  [Name("viasfora.rainbow.qicontroller")]
  [ContentType("text")]
  public class RainbowQuickInfoControllerProvider : IIntellisenseControllerProvider {
    [Import]
    internal IQuickInfoBroker QuickInfoBroker { get; set; }
    [Import]
    internal IViewTagAggregatorFactoryService AggregatorFactory { get; set; }
    [Import]
    internal IToolTipWindowProvider ToolTipProvider { get; set; }

    public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers) {
      return new RainbowQuickInfoController(textView, subjectBuffers, this);
    }
  }

  public class RainbowQuickInfoController : IIntellisenseController {
    private RainbowQuickInfoControllerProvider provider;
    private ITextView textView;
    private IList<ITextBuffer> subjectBuffers;
    private IQuickInfoSession session;
    private ITagAggregator<RainbowTag> aggregator;
    private IToolTipWindow toolTipWindow;

    public RainbowQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, RainbowQuickInfoControllerProvider provider) {
      this.textView = textView;
      this.subjectBuffers = subjectBuffers;
      this.provider = provider;

      this.textView.MouseHover += OnMouseHover;
      this.aggregator = 
        this.provider.AggregatorFactory.CreateTagAggregator<RainbowTag>(textView);
    }

    public void Detach(ITextView textView) {
      if ( this.textView == textView ) {
        this.textView.MouseHover -= this.OnMouseHover;
        this.textView = null;
        if ( this.toolTipWindow != null ) {
          this.toolTipWindow.Dispose();
          this.toolTipWindow = null;
        }
      }
    }

    public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) {
      subjectBuffers.Add(subjectBuffer);
    }

    public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
      subjectBuffers.Remove(subjectBuffer);
    }

    private void OnMouseHover(object sender, MouseHoverEventArgs e) {
      SnapshotPoint mousePos = new SnapshotPoint(e.View.TextSnapshot, e.Position);

      // Check to see if there is a RainbowTag under the Mouse
      // and if the tag is a closing brace
      var span = new SnapshotSpan(mousePos, 0);
      var tagSpan = aggregator.GetTags(span).FirstOrDefault();
      if ( tagSpan != null ) {
        char ch = mousePos.GetChar();
        LanguageInfo lang = VsfPackage.LookupLanguage(mousePos.Snapshot.ContentType);
        if ( lang == null )
          return;
        PresentQuickInfo(e.View, mousePos);
      }
    }

    private void PresentQuickInfo(ITextView view, SnapshotPoint mousePos) {
      SnapshotPoint bufferPos;
      if ( !RainbowProvider.TryMapPosToBuffer(view, mousePos, out bufferPos) ) {
        return;
      }
      ITrackingPoint triggerPoint = bufferPos.Snapshot.CreateTrackingPoint(
        bufferPos.Position, PointTrackingMode.Positive);

      if ( !provider.QuickInfoBroker.IsQuickInfoActive(view) ) {
        session = provider.QuickInfoBroker.CreateQuickInfoSession(view, triggerPoint, true);
        session.Dismissed += OnSessionDismissed;

        toolTipWindow = this.provider.ToolTipProvider.CreateToolTip(view);
        toolTipWindow.SetSize(60, 5);

        session.Set(toolTipWindow);
        session.Start();
      }
    }

    private void OnSessionDismissed(object sender, EventArgs e) {
      if ( this.session != null ) {
        this.session.Dismissed -= OnSessionDismissed;
        if ( this.toolTipWindow != null ) {
          this.toolTipWindow.Dispose();
          this.toolTipWindow = null;
        }
      }
    }
  }
}
