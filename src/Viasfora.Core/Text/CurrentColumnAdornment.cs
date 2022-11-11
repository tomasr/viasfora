using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Threading;
using System.Windows;

namespace Winterdom.Viasfora.Text {
  public class CurrentColumnAdornment {
    public const String CUR_COL_TAG = "currentColumn";
    private IAdornmentLayer layer;
    private IWpfTextView view;
    private IClassificationFormatMap formatMap;
    private IClassificationType formatType;
    private IVsfSettings settings;
    private Border highlight;
    private Dispatcher dispatcher;

    public CurrentColumnAdornment(
          IWpfTextView view, IClassificationFormatMap formatMap,
          IClassificationType formatType, IVsfSettings settings) {
      this.view = view;
      this.formatMap = formatMap;
      this.formatType = formatType;
      this.settings = settings;
      this.dispatcher = Dispatcher.CurrentDispatcher;
      this.highlight = new Border();
      this.layer = view.GetAdornmentLayer(Constants.COLUMN_HIGHLIGHT);

      view.Caret.PositionChanged += OnCaretPositionChanged;
      view.ViewportWidthChanged += OnViewportChanged;
      view.ViewportHeightChanged += OnViewportChanged;
      view.LayoutChanged += OnViewLayoutChanged;
      view.TextViewModel.EditBuffer.PostChanged += OnBufferPostChanged;
      view.Closed += OnViewClosed;
      view.Options.OptionChanged += OnSettingsChanged;

      this.settings.SettingsChanged += OnSettingsChanged;
      formatMap.ClassificationFormatMappingChanged +=
         OnClassificationFormatMappingChanged;

      CreateDrawingObjects();
    }

    void OnSettingsChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        this.UpdateViewOnUIThread();
      }
    }

    void UpdateViewOnUIThread() {
      var dispatcher = Dispatcher.CurrentDispatcher;
      if ( !dispatcher.CheckAccess() ) {
        Action action = this.UpdateView;
        dispatcher.Invoke(action);
      } else {
        this.UpdateView();
      }
    }

    void UpdateView() {
      CreateDrawingObjects();
      RedrawAdornments();
    }

    void OnClassificationFormatMappingChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        // the user changed something in Fonts and Colors, so
        // recreate our adornments
        CreateDrawingObjects();
        RedrawAdornments();
      }
    }
    void OnViewClosed(object sender, EventArgs e) {
      if ( this.settings != null ) {
        this.settings.SettingsChanged -= OnSettingsChanged;
      }
      if ( this.view != null ) {
        this.view.Caret.PositionChanged -= OnCaretPositionChanged;
        if ( this.view.Options != null ) {
          this.view.Options.OptionChanged -= OnSettingsChanged;
        }
        if ( this.view.TextViewModel?.EditBuffer != null ) {
          this.view.TextViewModel.EditBuffer.PostChanged -= OnBufferPostChanged;
        }
        this.view.ViewportWidthChanged -= OnViewportChanged;
        this.view.ViewportHeightChanged -= OnViewportChanged;
        this.view.Closed -= OnViewClosed;
        this.view.LayoutChanged -= OnViewLayoutChanged;
        this.view = null;
      }

      if ( this.formatMap != null ) {
        this.formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
        this.formatMap = null;
      }
      this.formatType = null;
    }
    void OnViewportChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        RedrawAdornments();
      }
    }
    void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
      if ( e.NewPosition != e.OldPosition && this.view != null ) {
        this.layer.RemoveAllAdornments();
        this.CreateVisuals(e.NewPosition.VirtualBufferPosition);
      }
    }
    private void OnBufferPostChanged(object sender, EventArgs e) {
      if ( this.view != null ) {
        this.layer.RemoveAllAdornments();
        this.CreateVisuals(this.view.Caret.Position.VirtualBufferPosition);
      }
    }
    private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
      if ( this.view != null && e.VerticalTranslation ) {
        this.layer.RemoveAllAdornments();
        this.CreateVisuals(this.view.Caret.Position.VirtualBufferPosition);
      }
    }

    private void CreateDrawingObjects() {
      // this gets the color settings configured by the
      // user in Fonts and Colors (or the default in out
      // classification type).
      TextFormattingRunProperties format =
         this.formatMap.GetExplicitTextProperties(this.formatType);

      this.highlight.BorderBrush = format.ForegroundBrush;
      switch ( this.settings.CurrentColumnHighlightStyle ) {
        case ColumnStyle.LeftBorder:
          this.highlight.BorderThickness = new Thickness(this.settings.HighlightLineWidth, 0, 0, 0);
          break;
        case ColumnStyle.RightBorder:
          this.highlight.BorderThickness = new Thickness(0, 0, this.settings.HighlightLineWidth, 0);
          break;
        default:
          this.highlight.BorderThickness = new Thickness(this.settings.HighlightLineWidth);
          break;
      }
      //this.border.BorderThickness = settings.HighlightLineWidth;
      var fill = new Rectangle();
      this.highlight.Child = fill;
      fill.Fill = format.BackgroundBrush;
      fill.StrokeThickness = 0;
    }
    private void RedrawAdornments() {
      if ( !this.view.InLayout && this.view.TextViewLines != null ) {
        this.layer.RemoveAllAdornments();
        var caret = this.view.Caret.Position;
        this.CreateVisuals(caret.VirtualBufferPosition);
      }
    }
    private void CreateVisuals(VirtualSnapshotPoint caretPosition) {
      if ( !this.settings.CurrentColumnHighlightEnabled ) {
        return; // not enabled
      }
      IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;
      if ( textViewLines == null )
        return; // not ready yet.
      // make sure the caret position is on the right buffer snapshot
      if ( caretPosition.Position.Snapshot != this.view.TextBuffer.CurrentSnapshot )
        return;

      var line = this.view.GetTextViewLineContainingBufferPosition(
        caretPosition.Position
        );
      var charBounds = line.GetCharacterBounds(caretPosition);

      this.highlight.Width = charBounds.Width;
      this.highlight.Height = this.view.ViewportHeight;
      if ( this.highlight.Height > 2 ) {
        this.highlight.Height -= 2;
      }

      // Align the image with the top of the bounds of the text geometry
      Canvas.SetLeft(this.highlight, charBounds.Left);
      Canvas.SetTop(this.highlight, this.view.ViewportTop);

      this.layer.AddAdornment(
         AdornmentPositioningBehavior.OwnerControlled, null,
         CUR_COL_TAG, this.highlight, null
      );
    }
  }
}
