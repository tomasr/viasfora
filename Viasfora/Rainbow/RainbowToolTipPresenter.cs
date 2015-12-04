using System;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Design;

namespace Winterdom.Viasfora.Rainbow {
  [Export(typeof(IIntellisensePresenterProvider))]
  [Name("viasfora.rainbow.tooltip.presenter")]
  [Order(Before="Default Quick Info Presenter")]
  [ContentType(ContentTypes.Text)]
  public class RainbowToolTipPresenterProvider : IIntellisensePresenterProvider {
    [Import]
    public ITextEditorFactoryService EditorFactory { get; set; }
    [Import]
    public IEditorOptionsFactoryService OptionsFactory { get; set; }

    public IIntellisensePresenter TryCreateIntellisensePresenter(IIntellisenseSession session) {
      IQuickInfoSession qiSession = session as IQuickInfoSession;
      if ( qiSession != null ) {
        if ( qiSession.Get<RainbowToolTipContext>() != null ) {
          return new RainbowToolTipPresenter(qiSession, this);
        }
      }
      return null;
    }
  }

  public class RainbowToolTipPresenter : IPopupIntellisensePresenter, IIntellisenseCommandTarget {
    private IQuickInfoSession session;
    private ITrackingSpan trackingSpan;
    private QuickInfoPresenter presenter;

    public IIntellisenseSession Session {
      get { return session; }
    }
    public double Opacity {
      get { return this.presenter.Opacity; }
      set { this.presenter.Opacity = value; }
    }
    public PopupStyles PopupStyles { get; private set; }
    public ITrackingSpan PresentationSpan {
      get {
        if ( this.trackingSpan == null ) {
          this.trackingSpan = GetPresentationSpan();
        }
        return this.trackingSpan;
      }
    }
    public string SpaceReservationManagerName { get; private set; }
    public UIElement SurfaceElement {
      get { return this.presenter; }
    }

#pragma warning disable 0067
    public event EventHandler SurfaceElementChanged;
    public event EventHandler PresentationSpanChanged;
    public event EventHandler<ValueChangedEventArgs<PopupStyles>> PopupStylesChanged;
#pragma warning restore 0067

    public RainbowToolTipPresenter(IQuickInfoSession qiSession, RainbowToolTipPresenterProvider provider) {
      this.session = qiSession;
      this.session.Dismissed += OnSessionDimissed;

      this.presenter = new QuickInfoPresenter();
      this.presenter.EditorFactory = provider.EditorFactory;
      this.presenter.OptionsFactory = provider.OptionsFactory;

      this.presenter.Opacity = 1.0;
      this.presenter.SnapsToDevicePixels = true;
      this.presenter.BindToSource(qiSession.QuickInfoContent);
      this.PopupStyles = PopupStyles.DismissOnMouseLeaveText
        | PopupStyles.PositionClosest;
      // This part is the key to making this work
      // so that the default quick info implementation
      // actually picks this up and displays our control
      this.SpaceReservationManagerName = "quickinfo";
    }

    private ITrackingSpan GetPresentationSpan() {
      return this.session.ApplicableToSpan;
    }

    public bool ExecuteKeyboardCommand(IntellisenseKeyboardCommand command) {
      switch ( command ) {
        case IntellisenseKeyboardCommand.Escape:
          if ( this.session != null ) {
            this.session.Dismiss();
            return true;
          }
          break;
      }
      return false;
    }

    private void OnSessionDimissed(object sender, EventArgs e) {
      this.session.Dismissed -= this.OnSessionDimissed;
      if ( this.presenter != null ) {
        this.presenter.Close();
      }
    }
  }
}
