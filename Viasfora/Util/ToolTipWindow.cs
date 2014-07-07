using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Winterdom.Viasfora.Contracts;
using System.Windows.Controls;

namespace Winterdom.Viasfora.Util {
  [Export(typeof(IToolTipWindowProvider))]
  public class ToolTipWindowProvider : IToolTipWindowProvider {
    [Import]
    public ITextEditorFactoryService EditorFactory { get; set; }
    [Import]
    public IScrollMapFactoryService ScrollMapFactory { get; set; }

    public IToolTipWindow CreateToolTip(ITextView textView) {
      return new ToolTipWindow(textView, this);
    }
  }

  public class ToolTipWindow : IToolTipWindow {
    private ITextView sourceTextView;
    private ToolTipWindowProvider provider;

    public ToolTipWindow(ITextView source, ToolTipWindowProvider provider) {
      this.sourceTextView = source;
      this.provider = provider;
    }

    public UIElement Show(int lineNumber, Size windowSize) {
      var block = new TextBlock();
      block.Text = "This is a test";
      return block;
    }
    public void Close() {
    }
  }
}
