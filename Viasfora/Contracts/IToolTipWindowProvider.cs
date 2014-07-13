using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Contracts {
  public interface IToolTipWindowProvider {
    IToolTipWindow CreateToolTip(ITextView textView);
  }
}
