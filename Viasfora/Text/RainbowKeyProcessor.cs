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
    public const long PRESS_TIME = 500;
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
      RainbowAdornment adornment = RainbowAdornment.Get(this.theView);
      adornment.Start();
    }

    private void StopRainbowAdornment() {
      RainbowAdornment adornment = RainbowAdornment.Get(this.theView);
      adornment.Stop();
    }
  }
}
