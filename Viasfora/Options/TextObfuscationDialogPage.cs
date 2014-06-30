using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Winterdom.Viasfora.Util;

namespace Winterdom.Viasfora.Options {
  public partial class TextObfuscationDialogPage : UserControl {
    public List<RegexEntry> Expressions { get; set; }
    public TextObfuscationDialogPage() {
      InitializeComponent();
      this.kindDataGridViewTextBoxColumn.DataSource = Enum.GetValues(typeof(ExpressionKind));
      this.optionsDataGridViewTextBoxColumn.DataSource = Enum.GetValues(typeof(ExpressionOptions));
    }

    public void DataLoaded() {
      this.listBindingSource.DataSource = this.Expressions;
    }
  }
}
