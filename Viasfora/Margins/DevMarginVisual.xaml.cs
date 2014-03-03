using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Winterdom.Viasfora.Margins {
  /// <summary>
  /// Interaction logic for DevMarginVisual.xaml
  /// </summary>
  public partial class DevMarginVisual : UserControl {

    public DevMarginViewModel Model { get; private set; }
    public event EventHandler ViewBuffer;

    public DevMarginVisual() {
      InitializeComponent();
      SetResources();
    }
    public DevMarginVisual(DevMarginViewModel model) : this() {
      this.Model = model;
      this.DataContext = Model;
    }

    private void OnViewBufferClick(object sender, RequestNavigateEventArgs e) {
      if ( ViewBuffer != null )
        ViewBuffer(this, EventArgs.Empty);
      e.Handled = true;
    }

    private void SetResources() {
      this.BindResource(TextBlock.ForegroundProperty, "ToolWindowTextBrushKey");
      this.BindResource(UserControl.BackgroundProperty, "ToolWindowBackgroundBrushKey");
      this.BindResource(ComboBox.BackgroundProperty, "ComboBoxBackgroundBrushKey");
      this.BindResource(ComboBox.BorderBrushProperty, "ComboBoxBorderBrushKey");
      this.BindResource(Control.BackgroundProperty, "ComboBoxPopupBackgroundGradientBrushKey");
      this.BindResource(ListBox.BorderBrushProperty, "ComboBoxPopupBorderBrushKey");
      this.BindResource(ComboBoxItem.ForegroundProperty, "ComboBoxItemTextBrushKey");
    }
  }
}
