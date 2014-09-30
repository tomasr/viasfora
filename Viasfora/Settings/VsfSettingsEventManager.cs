using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Winterdom.Viasfora.Settings {
  public class VsfSettingsEventManager : WeakEventManager {
    public static void AddListener(IVsfSettings source,
                                   IWeakEventListener handler) {
      if ( source == null )
        throw new ArgumentNullException("source");
      if ( handler == null )
        throw new ArgumentNullException("handler");

      CurrentManager.ProtectedAddListener(source, handler);
    }

    public static void RemoveListener(IVsfSettings source,
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
      IVsfSettings typedSource = (IVsfSettings)source;
      typedSource.SettingsChanged += DeliverEvent;
    }

    protected override void StopListening(object source) {
      IVsfSettings typedSource = (IVsfSettings)source;
      typedSource.SettingsChanged -= DeliverEvent;
    }

  }
}
