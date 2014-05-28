using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Util;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Text {

  [Export(typeof(IKeyProcessorProvider))]
  [Name("viasfora.rainbow.key.provider")]
  [Order(After="Default")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  [ContentType("text")]
  public class RainbowKeyProcessorProvider : IKeyProcessorProvider {

    public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView) {
      return new RainbowKeyProcessor(wpfTextView);
    }
  }

  public class RainbowKeyProcessor : KeyProcessor {
    private readonly ITextView theView;
    private Stopwatch timer = new Stopwatch();
    private bool startedEffect = false;
    private TimeSpan pressTime;
    public RainbowKeyProcessor(ITextView textView) {
      this.theView = textView;
      this.theView.LostAggregateFocus += OnLostFocus;
      this.theView.Closed += OnViewClosed;
      pressTime = TimeSpan.FromMilliseconds(VsfSettings.RainbowCtrlTimer);
    }

    // Strange things:
    // If a Peek-Definition window is opened, and the user
    // holds the key down on it, the main textview will
    // get the event, instead of the embedded window.
    //
    // I guess I'm probably doing something wrong,
    // such as not marking up this key processor correctly
    // so that the embedded window gets the event,
    // but for now, just use the event source
    // as a work around.
    public override void PreviewKeyDown(KeyEventArgs args) {
      ITextView actualView = GetViewFromEvent(args);
      if ( args.Key == Key.LeftCtrl ) {
        if ( timer.IsRunning ) {
          if ( timer.Elapsed >= pressTime ) {
            timer.Stop();
            RainbowHighlightMode mode = VsfSettings.RainbowHighlightMode;
            StartRainbowHighlight(actualView, mode);
          }
        } else {
          timer.Start();
        }
      } else {
        timer.Stop();
      }
    }

    private ITextView GetViewFromEvent(KeyEventArgs args) {
      ITextView view = args.OriginalSource as ITextView;
      return view ?? this.theView;
    }

    public override void PreviewKeyUp(KeyEventArgs args) {
      ITextView actualView = GetViewFromEvent(args);
      timer.Stop();
      StopRainbowHighlight(actualView);
    }

    private void OnViewClosed(object sender, EventArgs e) {
      this.theView.LostAggregateFocus -= OnLostFocus;
    }

    private void OnLostFocus(object sender, EventArgs e) {
      timer.Stop();
      StopRainbowHighlight(this.theView);
    }

    private void StartRainbowHighlight(ITextView view, RainbowHighlightMode mode) {
      if ( startedEffect ) return;
      startedEffect = true;

      SnapshotPoint bufferPos;
      if ( !TryMapCaretToBuffer(view, out bufferPos) ) {
        return;
      }

      ITextBuffer buffer = bufferPos.Snapshot.TextBuffer;
      RainbowProvider provider = buffer.Get<RainbowProvider>();
      if ( provider == null ) {
        return;
      }
      var braces = provider.BraceCache.GetBracePairFromPosition(bufferPos, mode);
      if ( braces == null ) return;
      SnapshotPoint opening = braces.Item1.ToPoint(bufferPos.Snapshot);
      SnapshotPoint closing = braces.Item2.ToPoint(bufferPos.Snapshot);

      if ( TryMapToView(view, opening, out opening) 
        && TryMapToView(view, closing, out closing) ) {
        RainbowHighlight highlight = RainbowHighlight.Get(view);
        if ( highlight != null ) {
          highlight.Start(opening, closing, braces.Item1.Depth);
        }
      }
    }

    private void StopRainbowHighlight(ITextView view) {
      RainbowHighlight highlight = RainbowHighlight.Get(view);
      if ( highlight != null ) {
        highlight.Stop();
      }
      startedEffect = false;
    }

    private bool TryMapToView(ITextView view, SnapshotPoint pos, out SnapshotPoint result) {
      result = new SnapshotPoint();
      var target = view.TextBuffer;
      var temp = view.BufferGraph.MapUpToBuffer(
        pos, PointTrackingMode.Negative,
        PositionAffinity.Successor, target
      );
      if ( temp != null ) {
        result = temp.Value;
        return true;
      }
      return false;
    }

    private bool TryMapCaretToBuffer(ITextView view, out SnapshotPoint pos) {
      var caret = view.Caret.Position.BufferPosition;
      pos = new SnapshotPoint();
      var result = view.BufferGraph.MapDownToFirstMatch(
        caret, PointTrackingMode.Negative,
        snapshot => snapshot.TextBuffer.Has<RainbowProvider>(),
        PositionAffinity.Successor
        );
      if ( result != null ) {
        pos = result.Value;
        return true;
      }
      return false;
    }

  }
}
