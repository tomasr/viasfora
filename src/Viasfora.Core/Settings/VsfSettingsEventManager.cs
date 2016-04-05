using System;
using System.Windows;

namespace Winterdom.Viasfora.Settings {
  public class VsfSettingsEventManager : WeakEventManager {
    public static void AddListener(IUpdatableSettings source,
                                   IWeakEventListener handler) {
      if ( source == null )
        throw new ArgumentNullException("source");
      if ( handler == null )
        throw new ArgumentNullException("handler");

      CurrentManager.ProtectedAddListener(source, handler);
    }

    public static void RemoveListener(IUpdatableSettings source,
                                      IWeakEventListener handler) {
      if ( source == null )
        throw new ArgumentNullException("source");
      if ( handler == null )
        throw new ArgumentNullException("handler");

      CurrentManager.ProtectedRemoveListener(source, handler);
    }

    private static VsfSettingsEventManager CurrentManager {
      get {
        Type managerType = typeof(VsfSettingsEventManager);
        VsfSettingsEventManager manager =
            (VsfSettingsEventManager)GetCurrentManager(managerType);

        // at first use, create and register a new manager
        if ( manager == null ) {
          manager = new VsfSettingsEventManager();
          SetCurrentManager(managerType, manager);
        }

        return manager;
      }
    }

    protected override void StartListening(object source) {
      var typedSource = (IUpdatableSettings)source;
      typedSource.SettingsChanged += DeliverEvent;
    }

    protected override void StopListening(object source) {
      var typedSource = (IUpdatableSettings)source;
      typedSource.SettingsChanged -= DeliverEvent;
    }

  }
}
