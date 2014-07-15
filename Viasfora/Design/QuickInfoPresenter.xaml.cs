using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Winterdom.Viasfora.Design {
  /// <summary>
  /// Interaction logic for QuickInfoPresenterxaml.xaml
  /// </summary>
  public partial class QuickInfoPresenter : UserControl {
    public ObservableCollection<object> DataSource { get; private set; }
    public QuickInfoPresenter() {
      InitializeComponent();
    }

    public void BindToSource(BulkObservableCollection<object> source) {
      this.DataSource = source;
      this.DataContext = this.DataSource;
    }
  }
}
