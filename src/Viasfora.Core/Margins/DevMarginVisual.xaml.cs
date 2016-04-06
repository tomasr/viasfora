using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Winterdom.Viasfora.Margins {
  /// <summary>
  /// Interaction logic for DevMarginVisual.xaml
  /// </summary>
  public partial class DevMarginVisual : UserControl {

    public DevMarginViewModel Model { get; private set; }
    private IVsfSettings settings;
    public event EventHandler ViewBuffer;

    public DevMarginVisual() {
      InitializeComponent();
    }
    public DevMarginVisual(DevMarginViewModel model, IVsfSettings settings) : this() {
      this.Model = model;
      this.DataContext = Model;
      this.settings = settings;
    }

    private void OnViewBufferClick(object sender, RoutedEventArgs e) {
      if ( ViewBuffer != null )
        ViewBuffer(this, EventArgs.Empty);
      e.Handled = true;
    }
    private void OnCloseButtonClick(object sender, RoutedEventArgs e) {
      settings.DeveloperMarginEnabled = false;
      settings.Save();
      e.Handled = true;
    }
    private void OnViewCTClick(object sender, RequestNavigateEventArgs e) {
      if ( Model.SelectedBuffer != null ) {
        this.ContentTypeTreePopup.BringIntoView();
      }
    }
  }
}
