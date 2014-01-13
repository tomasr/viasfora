using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Xml {
  internal class XmlQuickInfoController : IIntellisenseController {
    private ITextView textView;
    private IList<ITextBuffer> textBuffers;
    private IQuickInfoSession session;
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
      textBuffers.Add(subjectBuffer);
    }

    public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
      textBuffers.Remove(subjectBuffer);
    }
    void OnTextViewMouseHover(object sender, MouseHoverEventArgs e) {
      SnapshotPoint? point = textView.BufferGraph.MapDownToFirstMatch(
        new SnapshotPoint(textView.TextSnapshot, e.Position),
        PointTrackingMode.Positive,
        snapshot => textBuffers.Contains(snapshot.TextBuffer),
        PositionAffinity.Predecessor
      );
      if ( point != null ) {
        ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(
          point.Value.Position, PointTrackingMode.Positive);
        if ( provider.QuickInfoBroker.IsQuickInfoActive(textView) ) {
          session = provider.QuickInfoBroker.TriggerQuickInfo(textView, triggerPoint, true);
        }
      }
    }
  }
}
