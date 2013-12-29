using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winterdom.Viasfora.Design {
  public partial class ZoomTrackBar : UserControl {

    public int Value {
      get {
        return GetValue();
      }
      set {
        SetValue(value);
      }
    }
    public ZoomTrackBar() {
      InitializeComponent();
    }
    public int GetValue() {
      double trackbarValue = trackBar.Value;
      double zoomValue = (trackbarValue * trackbarValue) / (50 * 50);
      return (int)Math.Round(zoomValue);
    }
    public void SetValue(int zoomValue) {
      double trackbarValue = 50 * Math.Sqrt(zoomValue);
      trackBar.Value = (int)Math.Round(trackbarValue);
    }

    public void OnTrackBarValueChanged(object sender, EventArgs e) {
      label.Text = String.Format("{0} %", GetValue());
    }
  }
}
