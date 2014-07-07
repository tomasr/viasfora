using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Winterdom.Viasfora.Contracts {
  public interface IToolTipWindow {
    void Show(int lineNumber, Size windowSize);
    void Close();
  }
}
