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
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Options {
  public partial class TextObfuscationDialog : UserControl {
    public ObservableCollection<RegexEntry> Entries { get; private set; }
    public TextObfuscationDialog() {
      this.Entries = new ObservableCollection<RegexEntry>();
      InitializeComponent();
    }
  }
}
