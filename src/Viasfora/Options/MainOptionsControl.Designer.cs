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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.loadThemeButton = new System.Windows.Forms.Button();
      this.saveCurrentThemeButton = new System.Windows.Forms.Button();
      this.exportButton = new System.Windows.Forms.Button();
      this.importButton = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.importButton);
      this.groupBox1.Controls.Add(this.exportButton);
      this.groupBox1.Location = new System.Drawing.Point(4, 4);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(766, 178);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Export / Import";
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add(this.loadThemeButton);
      this.groupBox2.Controls.Add(this.saveCurrentThemeButton);
      this.groupBox2.Location = new System.Drawing.Point(7, 210);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(763, 178);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Themes";
      // 
      // loadThemeButton
      // 
      this.loadThemeButton.Location = new System.Drawing.Point(24, 90);
      this.loadThemeButton.Margin = new System.Windows.Forms.Padding(5);
      this.loadThemeButton.Name = "loadThemeButton";
      this.loadThemeButton.Size = new System.Drawing.Size(220, 48);
      this.loadThemeButton.TabIndex = 1;
      this.loadThemeButton.Text = "Load Theme";
      this.loadThemeButton.UseVisualStyleBackColor = true;
      this.loadThemeButton.Click += new System.EventHandler(this.LoadThemeButtonClick);
      // 
      // saveCurrentThemeButton
      // 
      this.saveCurrentThemeButton.Location = new System.Drawing.Point(24, 32);
      this.saveCurrentThemeButton.Margin = new System.Windows.Forms.Padding(5);
      this.saveCurrentThemeButton.Name = "saveCurrentThemeButton";
      this.saveCurrentThemeButton.Size = new System.Drawing.Size(220, 48);
      this.saveCurrentThemeButton.TabIndex = 0;
      this.saveCurrentThemeButton.Text = "Save Theme";
      this.saveCurrentThemeButton.UseVisualStyleBackColor = true;
      this.saveCurrentThemeButton.Click += new System.EventHandler(this.SaveCurrentThemeButtonClick);
      // 
      // exportButton
      // 
      this.exportButton.Location = new System.Drawing.Point(27, 32);
      this.exportButton.Margin = new System.Windows.Forms.Padding(5);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(194, 48);
      this.exportButton.TabIndex = 2;
      this.exportButton.Text = "Export Settings";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.ExportSettingsButtonClick);
      // 
      // importButton
      // 
      this.importButton.Location = new System.Drawing.Point(27, 90);
      this.importButton.Margin = new System.Windows.Forms.Padding(5);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(194, 48);
      this.importButton.TabIndex = 3;
      this.importButton.Text = "Import Settings";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.ImportSettingsButtonClick);
      // 
      // MainOptionsControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "MainOptionsControl";
      this.Size = new System.Drawing.Size(780, 470);
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Button loadThemeButton;
    private System.Windows.Forms.Button saveCurrentThemeButton;
    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.Button exportButton;
  }
}
