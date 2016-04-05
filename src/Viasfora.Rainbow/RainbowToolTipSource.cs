using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Microsoft.VisualStudio.Shell;

namespace Winterdom.Viasfora.Rainbow {

  [Export(typeof(IQuickInfoSourceProvider))]
  [Name("viasfora.rainbow.tooltip.source")]
  [ContentType(ContentTypes.Text)]
  public class RainbowToolTipSourceProvider : IQuickInfoSourceProvider {
    [Import]
    public IToolTipWindowProvider ToolTipProvider { get; set; }
    [Import]
    public IRainbowSettings Settings { get; set; }

    public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
      return new RainbowToolTipSource(textBuffer, this);
    }
  }

  public class RainbowToolTipSource : IQuickInfoSource {
    private ITextBuffer textBuffer;
    private RainbowToolTipSourceProvider provider;
    private IToolTipWindow toolTipWindow;

    public RainbowToolTipSource(ITextBuffer textBuffer, RainbowToolTipSourceProvider provider) {
      this.textBuffer = textBuffer;
      this.provider = provider;
    }

    public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
      applicableToSpan = null;
      if ( !provider.Settings.RainbowToolTipsEnabled ) {
        return;
      }
      SnapshotPoint? triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot);
      if ( !triggerPoint.HasValue ) {
        return;
      }

      SnapshotPoint? otherBrace;
      if ( !FindOtherBrace(triggerPoint.Value, out otherBrace) ) {
        // triggerPoint is not a brace
        return;
      }
      if ( !otherBrace.HasValue ) {
        TextEditor.DisplayMessageInStatusBar("No matching brace found.");
        return;
      }
      if ( IsTooClose(triggerPoint.Value, otherBrace.Value) ) {
        return;
      }
      session.Dismissed += OnSessionDismissed;

      if ( toolTipWindow == null ) {
        toolTipWindow = this.provider.ToolTipProvider.CreateToolTip(session.TextView);
        toolTipWindow.SetSize(60, 5);
      }

      var span = new SnapshotSpan(triggerPoint.Value, 1);
      applicableToSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgePositive);

      var element = toolTipWindow.GetWindow(otherBrace.Value);
      if ( element != null ) {
        quickInfoContent.Add(element);
        session.Set(new RainbowToolTipContext());
      }
    }

    private void OnSessionDismissed(object sender, EventArgs e) {
      IQuickInfoSession session = (IQuickInfoSession)sender;
      session.Dismissed -= OnSessionDismissed;
      if ( this.toolTipWindow != null ) {
        this.toolTipWindow.Dispose();
        this.toolTipWindow = null;
      }
    }

    private bool IsTooClose(SnapshotPoint point1, SnapshotPoint point2) {
      int distance = Math.Abs(point1 - point2);
      return distance < 100;
    }

    // returns true if brace is actually a brace.
    private bool FindOtherBrace(SnapshotPoint possibleBrace, out SnapshotPoint? otherBrace) {
      otherBrace = null;
      var rainbow = this.textBuffer.Get<RainbowProvider>();
      if ( rainbow == null ) {
        return false;
      }
      if ( !possibleBrace.IsValid() ) {
        return false;
      }

      if ( !rainbow.BufferBraces.BraceChars.Contains(possibleBrace.GetChar()) ) {
        return false;
      }
      var bracePair = rainbow.BufferBraces.GetBracePair(possibleBrace);
      if ( bracePair == null ) {
        return true;
      }
      if ( possibleBrace.Position == bracePair.Item1.Position ) {
        otherBrace = bracePair.Item2.ToPoint(possibleBrace.Snapshot);
      } else {
        otherBrace = bracePair.Item1.ToPoint(possibleBrace.Snapshot);
      }
      return true;
    }

    public void Dispose() {
    }
  }
}
