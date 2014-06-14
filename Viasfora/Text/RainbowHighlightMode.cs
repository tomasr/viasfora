using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Text {
  public enum RainbowHighlightMode {
    /// <summary>
    /// The caret point is used as the "insertion" point
    /// to track what pair of braces to highlight
    /// </summary>
    TrackInsertionPoint = 0,
    /// <summary>
    /// If the caret is next to an opening brace,
    /// the scope beginning with that will be highlighted.
    /// Otherwise, the default behavior applies. 
    /// </summary>
    TrackNextScope = 1,
  }
}
