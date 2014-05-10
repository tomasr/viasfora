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
  [TextViewRole(PredefinedTextViewRoles.Editable)]
  [ContentType("text")]
  public class RainbowKeyProcessorProvider : IKeyProcessorProvider {

    public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView) {
      return new RainbowKeyProcessor(wpfTextView);
    }
  }

  public class RainbowKeyProcessor : KeyProcessor {
    private ITextView theView;
    private Stopwatch timer = new Stopwatch();
    private bool startedEffect = false;
    private TimeSpan pressTime;
    public RainbowKeyProcessor(ITextView textView) {
      this.theView = textView;
      pressTime = TimeSpan.FromMilliseconds(VsfSettings.RainbowCtrlTimer);
    }

    public override void KeyDown(KeyEventArgs args) {
      if ( args.Key == Key.LeftCtrl ) {
        if ( timer.IsRunning ) {
          if ( timer.Elapsed >= pressTime ) {
            timer.Stop();
            StartRainbowAdornment();
          }
        } else {
          timer.Start();
        }
      } else {
        timer.Stop();
      }
    }

    public override void KeyUp(KeyEventArgs args) {
      timer.Stop();
      StopRainbowAdornment();
    }

    private void StartRainbowAdornment() {
      if ( startedEffect ) return;
      startedEffect = true;

      SnapshotPoint bufferPos;
      if ( !TryMapCaretToBuffer(out bufferPos) ) {
        return;
      }

      ITextBuffer buffer = bufferPos.Snapshot.TextBuffer;
      RainbowProvider provider = buffer.Get<RainbowProvider>();
      if ( provider == null ) {
        return;
      }
      var braces = provider.BraceCache.GetBracesAround(bufferPos);
      if ( braces == null ) return;
      SnapshotPoint opening = braces.Item1.ToPoint(bufferPos.Snapshot);
      SnapshotPoint closing = braces.Item2.ToPoint(bufferPos.Snapshot);

      if ( TryMapToView(opening, out opening) 
        && TryMapToView(closing, out closing) ) {
        RainbowHighlight adornment = RainbowHighlight.Get(this.theView);
        adornment.Start(opening, closing, braces.Item1.Depth);
      }
    }

    private bool TryMapToView(SnapshotPoint pos, out SnapshotPoint result) {
      result = new SnapshotPoint();
      var target = this.theView.TextBuffer;
      var temp = this.theView.BufferGraph.MapUpToBuffer(
        pos, PointTrackingMode.Negative,
        PositionAffinity.Successor, target
      );
      if ( temp != null ) {
        result = temp.Value;
        return true;
      }
      return false;
    }

    private bool TryMapCaretToBuffer(out SnapshotPoint pos) {
      var caret = this.theView.Caret.Position.BufferPosition;
      pos = new SnapshotPoint();
      var result = this.theView.BufferGraph.MapDownToFirstMatch(
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

    private void StopRainbowAdornment() {
      RainbowHighlight adornment = RainbowHighlight.Get(this.theView);
      adornment.Stop();
      startedEffect = false;
    }
  }
}
