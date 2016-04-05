using System;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Contracts {
  public interface IToolTipWindow : IDisposable {
    void SetSize(int widthChars, int heightChars);
    object GetWindow(SnapshotPoint bufferPosition);
  }
}
