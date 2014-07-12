using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Winterdom.Viasfora.Contracts;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IQuickInfoSourceProvider))]
  [Name("viasfora.rainbow.qisource")]
  [ContentType("text")]
  public class RainbowQuickInfoSourceProvider : IQuickInfoSourceProvider {

    public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
      return new RainbowQuickInfoSource(textBuffer, this);
    }
  }

  public class RainbowQuickInfoSource : IQuickInfoSource {
    private ITextBuffer textBuffer;
    private RainbowQuickInfoSourceProvider provider;

    public RainbowQuickInfoSource(ITextBuffer textBuffer, RainbowQuickInfoSourceProvider provider) {
      this.textBuffer = textBuffer;
      this.provider = provider;
    }

    public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
      applicableToSpan = null;
      SnapshotPoint? triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot);
      if ( !triggerPoint.HasValue ) {
        return;
      }

      SnapshotPoint? otherBrace = FindOtherBrace(triggerPoint.Value);
      if ( otherBrace == null || IsTooClose(triggerPoint.Value, otherBrace.Value) ) {
        return;
      }

      var toolTipWindow = session.Get<IToolTipWindow>();
      if ( toolTipWindow != null ) {
        var span = new SnapshotSpan(triggerPoint.Value, 1);
        applicableToSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgePositive);

        var element = toolTipWindow.GetWindow(otherBrace.Value);
        if ( element != null ) {
          quickInfoContent.Add(element);
        }
      }
    }

    private bool IsTooClose(SnapshotPoint point1, SnapshotPoint point2) {
      int distance = Math.Abs(point1 - point2);
      return distance < 100;
    }

    private SnapshotPoint? FindOtherBrace(SnapshotPoint brace) {
      var rainbow = this.textBuffer.Get<RainbowProvider>();
      if ( rainbow == null ) {
        return null;
      }

      var bracePair = rainbow.BraceCache.GetBracePair(brace);
      if ( bracePair == null ) {
        return null;
      }
      if ( brace.Position == bracePair.Item1.Position ) {
        return bracePair.Item2.ToPoint(brace.Snapshot);
      } else {
        return bracePair.Item1.ToPoint(brace.Snapshot);
      }
    }

    public void Dispose() {
    }
  }
}
