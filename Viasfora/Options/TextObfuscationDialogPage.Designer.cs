namespace Winterdom.Viasfora.Options {
  partial class TextObfuscationDialogPage {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.expressionsGridView = new System.Windows.Forms.DataGridView();
      this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.kindDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.optionsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.regularExpressionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.listBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.expressionsGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.listBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.expressionsGridView);
      this.groupBox1.Location = new System.Drawing.Point(17, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(1018, 665);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Obfuscation Expressions";
      // 
      // expressionsGridView
      // 
      this.expressionsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.expressionsGridView.AutoGenerateColumns = false;
      this.expressionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.expressionsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.kindDataGridViewTextBoxColumn,
            this.optionsDataGridViewTextBoxColumn,
            this.regularExpressionDataGridViewTextBoxColumn});
      this.expressionsGridView.DataSource = this.listBindingSource;
      this.expressionsGridView.Location = new System.Drawing.Point(18, 46);
      this.expressionsGridView.Name = "expressionsGridView";
      this.expressionsGridView.RowTemplate.Height = 33;
      this.expressionsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.expressionsGridView.Size = new System.Drawing.Size(980, 597);
      this.expressionsGridView.TabIndex = 0;
      this.expressionsGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellEndEdit);
      this.expressionsGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.OnCellValidating);
      // 
      // nameDataGridViewTextBoxColumn
      // 
      this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
      this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
      this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
      this.nameDataGridViewTextBoxColumn.Width = 180;
      // 
      // kindDataGridViewTextBoxColumn
      // 
      this.kindDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.kindDataGridViewTextBoxColumn.DataPropertyName = "Kind";
      this.kindDataGridViewTextBoxColumn.HeaderText = "Kind";
      this.kindDataGridViewTextBoxColumn.Name = "kindDataGridViewTextBoxColumn";
      this.kindDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.kindDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      this.kindDataGridViewTextBoxColumn.Width = 80;
      // 
      // optionsDataGridViewTextBoxColumn
      // 
      this.optionsDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.optionsDataGridViewTextBoxColumn.DataPropertyName = "Options";
      this.optionsDataGridViewTextBoxColumn.HeaderText = "Options";
      this.optionsDataGridViewTextBoxColumn.Name = "optionsDataGridViewTextBoxColumn";
      this.optionsDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.optionsDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      this.optionsDataGridViewTextBoxColumn.Width = 111;
      // 
      // regularExpressionDataGridViewTextBoxColumn
      // 
      this.regularExpressionDataGridViewTextBoxColumn.DataPropertyName = "RegularExpression";
      this.regularExpressionDataGridViewTextBoxColumn.HeaderText = "RegularExpression";
      this.regularExpressionDataGridViewTextBoxColumn.Name = "regularExpressionDataGridViewTextBoxColumn";
      this.regularExpressionDataGridViewTextBoxColumn.Width = 550;
      // 
      // listBindingSource
      // 
      this.listBindingSource.DataSource = typeof(Winterdom.Viasfora.Util.RegexEntry);
      // 
      // TextObfuscationDialogPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Name = "TextObfuscationDialogPage";
      this.Size = new System.Drawing.Size(1072, 700);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.expressionsGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.listBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView expressionsGridView;
    private System.Windows.Forms.BindingSource listBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn kindDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn optionsDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn regularExpressionDataGridViewTextBoxColumn;
  }
}
