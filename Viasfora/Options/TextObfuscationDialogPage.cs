using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Winterdom.Viasfora.Util;
using System.Text.RegularExpressions;

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

    private void OnCellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      DataGridView view = (DataGridView)sender;
      if ( e.ColumnIndex == 0 ) {
        if ( String.IsNullOrWhiteSpace((String)e.FormattedValue) ) {
          view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Name cannot be empty";
        }
      } else if ( e.ColumnIndex == 3 ) {
        try {
          var regex = new Regex((String)e.FormattedValue);
        } catch {
          view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Regular expression is not valid";
        }
      }
    }

    private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e) {
      DataGridView view = (DataGridView)sender;
      view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
    }
  }
}
