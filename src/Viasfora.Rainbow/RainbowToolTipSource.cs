using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.Viasfora.Rainbow {

  [Export(typeof(IAsyncQuickInfoSourceProvider))]
  [Name("viasfora.rainbow.tooltip.source")]
  [Order(After = "Default")]
  [ContentType(ContentTypes.Text)]
  public class RainbowToolTipSourceProvider : IAsyncQuickInfoSourceProvider {
    [Import]
    public IToolTipWindowProvider ToolTipProvider { get; set; }
    [Import]
    public IRainbowSettings Settings { get; set; }

    public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
      return new RainbowToolTipSource(textBuffer, this);
    }
  }

  public sealed class RainbowToolTipSource : IAsyncQuickInfoSource {
    private ITextBuffer textBuffer;
    private RainbowToolTipSourceProvider provider;
    private IToolTipWindow toolTipWindow;
    private bool isRoslynBuffer;

    public RainbowToolTipSource(ITextBuffer textBuffer, RainbowToolTipSourceProvider provider) {
      this.textBuffer = textBuffer;
      this.provider = provider;
      this.isRoslynBuffer = textBuffer.ContentType.IsOfType(ContentTypes.Roslyn);
    }

    public async Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken) {
      ITrackingSpan applicableToSpan = null;
      QuickInfoItem result = null;
      if ( !this.provider.Settings.RainbowToolTipsEnabled || cancellationToken.IsCancellationRequested ) {
        return result;
      }
      SnapshotPoint? triggerPoint = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);
      if ( !triggerPoint.HasValue ) {
        return result;
      }

      SnapshotPoint? otherBrace;
      if ( !FindOtherBrace(triggerPoint.Value, out otherBrace) ) {
        // triggerPoint is not a brace
        return result;
      }
      if ( !otherBrace.HasValue ) {
        TextEditor.DisplayMessageInStatusBar("No matching brace found.");
        return result;
      }
      if ( IsTooClose(triggerPoint.Value, otherBrace.Value) ) {
        return result;
      }

      // It appears in VS2026, any attempt to dispose the tool window
      // will result in a StackOverflowException somewhere deep in VS code.
      // Given this, no point in attaching to the Dismissed event, since
      // we will crash if we attempt to do anything on it.
      //session.StateChanged += OnQuickInfoSessionStateChanged;

      var span = new SnapshotSpan(triggerPoint.Value, 1);
      applicableToSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgePositive);

      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

      if ( this.toolTipWindow == null ) {
        this.toolTipWindow = this.provider.ToolTipProvider.CreateToolTip(session.TextView);
      }
      this.toolTipWindow.SetSize(60, 5);

      var element = this.toolTipWindow.GetWindow(otherBrace.Value);
      if ( element != null ) {
        session.Set(new RainbowToolTipContext());
        result = new QuickInfoItem(applicableToSpan, element);
      }
      return result;
    }

    private void OnQuickInfoSessionStateChanged(object sender, QuickInfoSessionStateChangedEventArgs e) {
      if ( e.NewState == QuickInfoSessionState.Dismissed ) {
        IAsyncQuickInfoSession session = (IAsyncQuickInfoSession)sender;
        session.StateChanged -= OnQuickInfoSessionStateChanged;
        if ( this.toolTipWindow != null ) {
          this.toolTipWindow.Dispose();
          this.toolTipWindow = null;
        }
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
        return false;
      }
      // The cursor position is on the closing brace. Visual Studio already has 
      // decent support on Roslyn for showing tooltips on closing braces, so we just
      // ignore it
      if ( possibleBrace.Position == bracePair.Item1.Position ) {
        otherBrace = bracePair.Item2.ToPoint(possibleBrace.Snapshot);
        return true;
      } else if ( !this.isRoslynBuffer ) {
        otherBrace = bracePair.Item1.ToPoint(possibleBrace.Snapshot);
        return true;
      }
      return false;
    }

    public void Dispose() {
      this.textBuffer = null;
      if ( this.toolTipWindow != null ) {
        this.toolTipWindow.Dispose();
        this.toolTipWindow = null;
      }
    }
  }
}
