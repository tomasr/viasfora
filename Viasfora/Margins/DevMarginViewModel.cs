using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

namespace Winterdom.Viasfora.Margins {
  public class DevMarginViewModel : INotifyPropertyChanged {
    private ObservableCollection<BufferInfoViewModel> bufferGraph = new ObservableCollection<BufferInfoViewModel>();
    private ObservableCollection<String> textViewRoles = new ObservableCollection<string>();
    private String bufferPosition;
    private BufferInfoViewModel selectedBuffer;
    public ReadOnlyObservableCollection<BufferInfoViewModel> BufferGraph {
      get { return new ReadOnlyObservableCollection<BufferInfoViewModel>(bufferGraph); }
    }
    public String BufferPosition {
      get { return bufferPosition; }
      set { bufferPosition = value; NotifyChanged("BufferPosition"); }
    }
    public BufferInfoViewModel SelectedBuffer {
      get { return selectedBuffer; }
      set { selectedBuffer = value; NotifyChanged("SelectedBuffer"); }
    }
    public ObservableCollection<String> TextViewRoles {
      get { return this.textViewRoles; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void RefreshView(ITextView view) {
      this.TextViewRoles.Clear();
      foreach ( var role in view.Roles ) {
        this.TextViewRoles.Add(role);
      }
    }
    public void RefreshBuffers(IBufferGraph graph) {
      this.bufferGraph.Clear();
      var buffers = graph.GetTextBuffers(b => true);
      int index = 0;
      foreach ( var buffer in buffers ) {
        this.bufferGraph.Add(new BufferInfoViewModel {
          ContentType = buffer.ContentType.DisplayName,
          BufferType = buffer.GetType(),
          ActualContentType = new ContentTypeViewModel(buffer.ContentType),
          Index = index++
        });
      }
      NotifyChanged("BufferGraph");
      this.SelectedBuffer = this.bufferGraph.FirstOrDefault(
        b => TextEditor.IsNonProjectionOrElisionBufferType(b.BufferType));
    }
    private void NotifyChanged(String property) {
      if ( PropertyChanged != null ) {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }
  }

  public class BufferInfoViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    private String contentType;
    private Type bufferType;
    private int index;
    private ContentTypeViewModel ctViewModel;
    public String ContentType {
      get { return contentType; }
      set { contentType = value; NotifyChanged("ContentType"); }
    }
    public Type BufferType {
      get { return bufferType; }
      set { bufferType = value; NotifyChanged("BufferType"); }
    }
    public int Index {
      get { return index; }
      set { index = value; NotifyChanged("Index"); }
    }
    public ContentTypeViewModel ActualContentType {
      get { return ctViewModel; }
      set { ctViewModel = value; NotifyChanged("ActualContentType"); }
    }

    public String DisplayName {
      get { return String.Format("{0} ({1})", ContentType, BufferType.Name); }
    }

    private void NotifyChanged(String property) {
      if ( PropertyChanged != null ) {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }
  }
  public class ContentTypeViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    private String displayName;
    private ObservableCollection<ContentTypeViewModel> baseTypes;
    public String DisplayName {
      get { return displayName; }
      set { displayName = value; NotifyChanged("DisplayName"); }
    }
    public ObservableCollection<ContentTypeViewModel> BaseTypes {
      get { return baseTypes; }
      set { baseTypes = value; NotifyChanged("BaseTypes"); }
    }

    public ContentTypeViewModel(IContentType type) {
      this.DisplayName = type.DisplayName;
      var btCollection = new ObservableCollection<ContentTypeViewModel>();
      foreach ( var bt in type.BaseTypes ) {
        btCollection.Add(new ContentTypeViewModel(bt));
      }
      this.BaseTypes = btCollection;
    }
    private void NotifyChanged(String property) {
      if ( PropertyChanged != null ) {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }
  }
}
