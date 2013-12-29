namespace Winterdom.Viasfora.Options {
  partial class PresentationModeDialogPage {
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
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.defaultZoom = new Winterdom.Viasfora.Design.ZoomTrackBar();
      this.enabledZoom = new Winterdom.Viasfora.Design.ZoomTrackBar();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.enabledZoom);
      this.groupBox1.Controls.Add(this.defaultZoom);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(902, 367);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Presentation Mode";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(19, 160);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(215, 25);
      this.label2.TabIndex = 1;
      this.label2.Text = "Enabled Zoom Level:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(19, 46);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(204, 25);
      this.label1.TabIndex = 0;
      this.label1.Text = "Default Zoom Level:";
      // 
      // defaultZoom
      // 
      this.defaultZoom.Location = new System.Drawing.Point(24, 75);
      this.defaultZoom.Name = "defaultZoom";
      this.defaultZoom.Size = new System.Drawing.Size(607, 79);
      this.defaultZoom.TabIndex = 2;
      this.defaultZoom.Value = 0;
      // 
      // enabledZoom
      // 
      this.enabledZoom.Location = new System.Drawing.Point(24, 201);
      this.enabledZoom.Name = "enabledZoom";
      this.enabledZoom.Size = new System.Drawing.Size(607, 79);
      this.enabledZoom.TabIndex = 3;
      this.enabledZoom.Value = 0;
      // 
      // PresentationModeDialogPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Name = "PresentationModeDialogPage";
      this.Size = new System.Drawing.Size(902, 543);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private Design.ZoomTrackBar enabledZoom;
    private Design.ZoomTrackBar defaultZoom;
  }
}
