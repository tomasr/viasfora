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
    public const long PRESS_TIME = 300;
    private bool startedEffect = false;
    public RainbowKeyProcessor(ITextView textView) {
      this.theView = textView;
    }

    public override void KeyDown(KeyEventArgs args) {
      if ( args.Key == Key.LeftCtrl ) {
        if ( timer.IsRunning ) {
          if ( timer.ElapsedMilliseconds >= PRESS_TIME ) {
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
      RainbowProvider provider;
      if ( !buffer.Properties.TryGetProperty(typeof(RainbowProvider), out provider) ) {
        return;
      }
      Tuple<BracePos, BracePos> braces = provider.BraceCache.GetBracesAround(bufferPos);
      if ( braces == null ) return;
      SnapshotPoint opening = new SnapshotPoint(bufferPos.Snapshot, braces.Item1.Position);
      SnapshotPoint closing = new SnapshotPoint(bufferPos.Snapshot, braces.Item2.Position);

      if ( TryMapToView(opening, out opening) && TryMapToView(closing, out closing) ) {
        RainbowAdornment adornment = RainbowAdornment.Get(this.theView);
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
        snapshot => snapshot.TextBuffer.Properties.ContainsProperty(typeof(RainbowProvider)),
        PositionAffinity.Successor
        );
      if ( result != null ) {
        pos = result.Value;
        return true;
      }
      return false;
    }

    private void StopRainbowAdornment() {
      RainbowAdornment adornment = RainbowAdornment.Get(this.theView);
      adornment.Stop();
      startedEffect = false;
    }
  }
}
