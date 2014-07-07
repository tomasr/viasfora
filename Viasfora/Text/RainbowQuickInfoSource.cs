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
      SnapshotPoint? subjectTriggerPoint =
        session.GetTriggerPoint(textBuffer.CurrentSnapshot);
      if ( !subjectTriggerPoint.HasValue ) {
        return;
      }
      var toolTipWindow = session.Get<IToolTipWindow>();
      if ( toolTipWindow != null ) {
        var span = new SnapshotSpan(subjectTriggerPoint.Value, 1);
        applicableToSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgePositive);

        var element = toolTipWindow.Show(100, new Size(100, 100));
        quickInfoContent.Add(element);
      }
    }

    public void Dispose() {
    }
  }
}
