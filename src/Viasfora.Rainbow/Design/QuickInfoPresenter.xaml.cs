using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.Specialized;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Design {
  /// <summary>
  /// Interaction logic for QuickInfoPresenterxaml.xaml
  /// </summary>
  public partial class QuickInfoPresenter : UserControl {
    public ObservableCollection<object> DataSource { get; private set; }

    public IEditorOptionsFactoryService OptionsFactory { get; set; }
    public ITextEditorFactoryService EditorFactory { get; set; }
    private List<IWpfTextView> viewsToRelease;

    public QuickInfoPresenter() {
      InitializeComponent();
    }

    public void BindToSource(BulkObservableCollection<object> source) {
      this.viewsToRelease = new List<IWpfTextView>();
      this.DataSource = new ShadowedBOCollection(source, this.Map);
      this.DataContext = this.DataSource;
    }

    public void Close() {
      foreach ( var view in this.viewsToRelease ) {
        try {
          view.Close();
        } catch {
        }
      }
      this.viewsToRelease.Clear();
    }

    private UIElement Map(object entry) {
      if ( entry is IWpfTextView ) {
        Border border = new Border();
        border.Child = ((IWpfTextView)entry).VisualElement;
        return border;
      } else if ( entry is ITextBuffer ) {
        return CreateViewForBuffer((ITextBuffer)entry);
      } else if ( entry is String ) {
        var tb = new TextBlock();
        tb.Text = (String)entry;
        return tb;
      }
      return (UIElement)entry;
    }

    private UIElement CreateViewForBuffer(ITextBuffer entry) {
      var options = this.OptionsFactory.CreateOptions();
      var roles = this.EditorFactory.CreateTextViewRoleSet("");
      var view = this.EditorFactory.CreateTextView(entry, roles, options);

      // make the tooltip be formatted using the "Editor ToolTip" category
      view.Options.SetOptionValue(
        DefaultWpfViewOptions.AppearanceCategory,
        FontsAndColorsCategories.ToolTipFontAndColorCategory
      );

      view.Background = Brushes.Transparent;
      this.viewsToRelease.Add(view);

      Border border = new AutoSizeContainer();
      border.Background = Brushes.Transparent;
      border.Child = view.VisualElement;
      border.Margin = new Thickness(5, 3, 5, 3);
      return border;
    }
  }

  class AutoSizeContainer : Border {
    protected override Size MeasureOverride(Size constraint) {
      Size baseSize = base.MeasureOverride(constraint);
      Size desiredSize = baseSize; 
      IWpfTextView view = Child as IWpfTextView;
      if ( view != null ) {
        // this is an ugly hack, but not sure how
        // to actually ask the IWpfTextView
        // how much space it would need to render content
        desiredSize = new Size(
          desiredSize.Width,
          view.LineHeight * view.VisualSnapshot.LineCount + 
          this.BorderThickness.Top + this.BorderThickness.Bottom +
          this.Margin.Top + this.Margin.Bottom
          );
      }
      return desiredSize;
    }
  }

  class ShadowedBOCollection : BulkObservableCollection<object> {
    public delegate UIElement TranslateFunc(object entry);
    private BulkObservableCollection<object> source;
    private TranslateFunc translator;

    public ShadowedBOCollection(
          BulkObservableCollection<object> originalSource,
          TranslateFunc translation) {
      this.source = originalSource;
      this.source.CollectionChanged += OnSourceModified;
      this.translator = translation;
      UpdateFromSource();
    }

    private void OnSourceModified(object sender, NotifyCollectionChangedEventArgs e) {
      UpdateFromSource();
    }

    private void UpdateFromSource() {
      this.BeginBulkOperation();
      this.Clear();
      foreach ( object entry in this.source ) {
        this.Add(this.translator(entry));
      }
      this.EndBulkOperation();
    }
  }
}
