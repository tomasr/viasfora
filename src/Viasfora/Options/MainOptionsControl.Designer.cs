namespace Winterdom.Viasfora.Options {
  partial class MainOptionsControl {
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.importButton = new System.Windows.Forms.Button();
      this.exportButton = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.loadThemeButton = new System.Windows.Forms.Button();
      this.saveCurrentThemeButton = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Location = new System.Drawing.Point(4, 4);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(766, 203);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Export / Import";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.importButton, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.exportButton, 1, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 27);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(760, 173);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // importButton
      // 
      this.importButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.importButton.Location = new System.Drawing.Point(385, 25);
      this.importButton.Margin = new System.Windows.Forms.Padding(5);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(220, 42);
      this.importButton.TabIndex = 1;
      this.importButton.Text = "Import Settings";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.ImportSettingsButtonClick);
      // 
      // exportButton
      // 
      this.exportButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.exportButton.Location = new System.Drawing.Point(155, 25);
      this.exportButton.Margin = new System.Windows.Forms.Padding(5);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(220, 42);
      this.exportButton.TabIndex = 0;
      this.exportButton.Text = "Export Settings";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.ExportSettingsButtonClick);
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add(this.tableLayoutPanel2);
      this.groupBox2.Location = new System.Drawing.Point(7, 233);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(763, 178);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Themes";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 4;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.loadThemeButton, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.saveCurrentThemeButton, 1, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 27);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.Size = new System.Drawing.Size(757, 148);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // loadThemeButton
      // 
      this.loadThemeButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.loadThemeButton.Location = new System.Drawing.Point(383, 25);
      this.loadThemeButton.Margin = new System.Windows.Forms.Padding(5);
      this.loadThemeButton.Name = "loadThemeButton";
      this.loadThemeButton.Size = new System.Drawing.Size(220, 42);
      this.loadThemeButton.TabIndex = 1;
      this.loadThemeButton.Text = "Load Theme";
      this.loadThemeButton.UseVisualStyleBackColor = true;
      this.loadThemeButton.Click += new System.EventHandler(this.LoadThemeButtonClick);
      // 
      // saveCurrentThemeButton
      // 
      this.saveCurrentThemeButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.saveCurrentThemeButton.Location = new System.Drawing.Point(153, 25);
      this.saveCurrentThemeButton.Margin = new System.Windows.Forms.Padding(5);
      this.saveCurrentThemeButton.Name = "saveCurrentThemeButton";
      this.saveCurrentThemeButton.Size = new System.Drawing.Size(220, 42);
      this.saveCurrentThemeButton.TabIndex = 0;
      this.saveCurrentThemeButton.Text = "Save Theme";
      this.saveCurrentThemeButton.UseVisualStyleBackColor = true;
      this.saveCurrentThemeButton.Click += new System.EventHandler(this.SaveCurrentThemeButtonClick);
      // 
      // MainOptionsControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "MainOptionsControl";
      this.Size = new System.Drawing.Size(780, 618);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.Button exportButton;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button loadThemeButton;
    private System.Windows.Forms.Button saveCurrentThemeButton;
  }
}
