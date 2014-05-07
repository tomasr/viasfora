using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
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
      var caret = this.theView.Caret.Position.BufferPosition;
      var buffer = TextEditor.GetPrimaryBuffer(this.theView);
      RainbowProvider provider;
      if ( !buffer.Properties.TryGetProperty(typeof(RainbowProvider), out provider) ) {
        return;
      }
      Tuple<BracePos, BracePos> braces = provider.BraceCache.GetBracesAround(caret);
      if ( braces == null ) return;
      SnapshotPoint opening = new SnapshotPoint(caret.Snapshot, braces.Item1.Position);
      SnapshotPoint closing = new SnapshotPoint(caret.Snapshot, braces.Item2.Position);
      RainbowAdornment adornment = RainbowAdornment.Get(this.theView);
      adornment.Start(opening, closing);
    }

    private void StopRainbowAdornment() {
      RainbowAdornment adornment = RainbowAdornment.Get(this.theView);
      adornment.Stop();
      startedEffect = false;
    }
  }
}
