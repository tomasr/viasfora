using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Xml {
  internal class XmlQuickInfoController : IIntellisenseController {
    private ITextView textView;
    private IList<ITextBuffer> textBuffers;
    private XmlQuickInfoControllerProvider provider;

    internal XmlQuickInfoController(
        ITextView textView, IList<ITextBuffer> textBuffers,
        XmlQuickInfoControllerProvider provider) {
      this.textView = textView;
      this.textBuffers = textBuffers;
      this.provider = provider;

      textView.MouseHover += OnTextViewMouseHover;
    }
    public void Detach(ITextView textView) {
      if ( this.textView == textView ) {
        textView.MouseHover -= this.OnTextViewMouseHover;
        textView = null;
      }
    }
    public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) {
      this.textBuffers.Add(subjectBuffer);
    }

    public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
      this.textBuffers.Remove(subjectBuffer);
    }
    void OnTextViewMouseHover(object sender, MouseHoverEventArgs e) {
      SnapshotPoint? point = this.textView.BufferGraph.MapDownToFirstMatch(
        new SnapshotPoint(this.textView.TextSnapshot, e.Position),
        PointTrackingMode.Positive,
        snapshot => this.textBuffers.Contains(snapshot.TextBuffer),
        PositionAffinity.Predecessor
      );
      if ( point != null ) {
        ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(
          point.Value.Position, PointTrackingMode.Positive);
        if ( this.provider.QuickInfoBroker.IsQuickInfoActive(this.textView) ) {
          Task task = this.provider.QuickInfoBroker.TriggerQuickInfoAsync(this.textView, triggerPoint);
          // Can't wait until it's done. Just let it happen Async
        }
      }
    }
  }

}
