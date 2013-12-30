namespace Winterdom.Viasfora.Design {
  partial class ZoomTrackBar {
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
      this.trackBar = new System.Windows.Forms.TrackBar();
      this.label = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // trackBar
      // 
      this.trackBar.Dock = System.Windows.Forms.DockStyle.Left;
      this.trackBar.LargeChange = 10;
      this.trackBar.Location = new System.Drawing.Point(0, 0);
      this.trackBar.Maximum = 975;
      this.trackBar.Name = "trackBar";
      this.trackBar.Size = new System.Drawing.Size(489, 79);
      this.trackBar.SmallChange = 2;
      this.trackBar.TabIndex = 0;
      this.trackBar.TickFrequency = 50;
      this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.trackBar.ValueChanged += new System.EventHandler(this.OnTrackBarValueChanged);
      // 
      // label
      // 
      this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label.Location = new System.Drawing.Point(495, 0);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(112, 55);
      this.label.TabIndex = 1;
      this.label.Text = "100 %";
      this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // ZoomTrackBar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label);
      this.Controls.Add(this.trackBar);
      this.Name = "ZoomTrackBar";
      this.Size = new System.Drawing.Size(607, 79);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TrackBar trackBar;
    private System.Windows.Forms.Label label;
  }
}
