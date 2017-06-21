using System;
using Winterdom.Viasfora.Contracts;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  public abstract class LanguageSettings : SettingsBase, ILanguageSettings {
    protected static readonly String[] EMPTY = { };
    protected abstract String[] ControlFlowDefaults { get; }
    protected abstract String[] LinqDefaults { get; }
    protected abstract String[] VisibilityDefaults { get; }

    public String KeyName { get; private set; }
    public String[] ControlFlow {
      get { return this.Store.GetList(KeyName + "_ControlFlow", ControlFlowDefaults); }
      set { this.Store.SetValue(KeyName + "_ControlFlow", value); }
    }
    public String[] Linq {
      get { return this.Store.GetList(KeyName + "_Linq", LinqDefaults); }
      set { this.Store.SetValue(KeyName + "_Linq", value); }
    }
    public String[] Visibility {
      get { return this.Store.GetList(KeyName + "_Visibility", VisibilityDefaults); }
      set { this.Store.SetValue(KeyName + "_Visibility", value); }
    }
    public bool Enabled {
      get { return this.Store.GetBoolean(KeyName + "_Enabled", true); }
      set { this.Store.SetValue(KeyName + "_Enabled", value); }
    }

    public LanguageSettings(String key, ITypedSettingsStore store)
      : base(store) {
      this.KeyName = key;
    }
  }
}
