using System;

namespace Winterdom.Viasfora.Settings {
  public abstract class SettingsBase : IUpdatableSettings {
    protected ITypedSettingsStore Store { get; private set; }
    public event EventHandler SettingsChanged;

    public SettingsBase(ITypedSettingsStore store) {
      this.Store = store;
      this.Store.SettingsChanged += OnStoreChanged;
    }

    public void Load() {
      this.Store.Load();
    }

    public void Save() {
      this.Store.Save();
    }

    private void OnStoreChanged(object sender, EventArgs e) {
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
