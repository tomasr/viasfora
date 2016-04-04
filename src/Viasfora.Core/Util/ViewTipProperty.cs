using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Util {
  public class ViewTipProperty {
    public SnapshotPoint Position { get; private set; }
    public ViewTipProperty(SnapshotPoint position) {
      this.Position = position;
    }
  }
}
