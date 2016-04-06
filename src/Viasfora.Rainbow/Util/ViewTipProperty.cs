using Microsoft.VisualStudio.Text;
using System;

namespace Winterdom.Viasfora.Util {
  public class ViewTipProperty {
    public SnapshotPoint Position { get; private set; }
    public ViewTipProperty(SnapshotPoint position) {
      this.Position = position;
    }
  }
}
