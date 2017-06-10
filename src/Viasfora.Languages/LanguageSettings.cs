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
      get { return GetList(KeyName + "_ControlFlow", ControlFlowDefaults); }
      set { SetValue(KeyName + "_ControlFlow", value); }
    }
    public String[] Linq {
      get { return GetList(KeyName + "_Linq", LinqDefaults); }
      set { SetValue(KeyName + "_Linq", value); }
    }
    public String[] Visibility {
      get { return GetList(KeyName + "_Visibility", VisibilityDefaults); }
      set { SetValue(KeyName + "_Visibility", value); }
    }
    public bool Enabled {
      get { return GetBoolean(KeyName + "_Enabled", true); }
      set { SetValue(KeyName + "_Enabled", value); }
    }

    public LanguageSettings(String key, ISettingsStore store, IStorageConversions converter)
      : base(store, converter) {
      this.KeyName = key;
    }
  }
}
