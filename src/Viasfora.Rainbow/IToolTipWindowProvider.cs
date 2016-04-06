using System;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Rainbow {
  public interface IToolTipWindowProvider {
    IToolTipWindow CreateToolTip(ITextView textView);
  }
}
