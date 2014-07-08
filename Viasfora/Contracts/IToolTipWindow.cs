using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Contracts {
  public interface IToolTipWindow : IDisposable {
    void SetSize(int widthChars, int heightChars);
    UIElement GetWindow(SnapshotSpan span);
  }
}
