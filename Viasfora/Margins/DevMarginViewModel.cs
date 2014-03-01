using Microsoft.VisualStudio.Text.Projection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Margins {
  public class DevMarginViewModel : INotifyPropertyChanged {
    private ObservableCollection<BufferInfo> bufferGraph = new ObservableCollection<BufferInfo>();
    private int bufferPosition;
    private BufferInfo selectedBuffer;
    public ReadOnlyObservableCollection<BufferInfo> BufferGraph {
      get { return new ReadOnlyObservableCollection<BufferInfo>(bufferGraph); }
    }
    public int BufferPosition {
      get { return bufferPosition; }
      set { bufferPosition = value; NotifyChanged("BufferPosition"); }
    }
    public BufferInfo SelectedBuffer {
      get { return selectedBuffer; }
      set { selectedBuffer = value; NotifyChanged("SelectedBuffer"); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void RefreshBuffers(IBufferGraph graph) {
      this.bufferGraph.Clear();
      var buffers = graph.GetTextBuffers(b => true);
      int index = 0;
      foreach ( var buffer in buffers ) {
        this.bufferGraph.Add(new BufferInfo {
          ContentType = buffer.ContentType.DisplayName,
          BufferType = buffer.GetType(),
          Index = index++
        });
      }
      NotifyChanged("BufferGraph");
      this.SelectedBuffer = this.bufferGraph.FirstOrDefault(
        b => TextEditor.IsPrimaryBufferType(b.BufferType));
    }
    private void NotifyChanged(String property) {
      if ( PropertyChanged != null ) {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }

    public class BufferInfo {
      public String ContentType { get; set; }
      public Type BufferType { get; set; }
      public int Index { get; set; }

      public String DisplayName {
        get { return String.Format("{0} ({1})", ContentType, BufferType.Name); }
      }
    }
  }
}
