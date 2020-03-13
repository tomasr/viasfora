using System;
using System.Collections.Generic;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Languages {
  public abstract class LanguageSettings : SettingsBase, ILanguageSettings, ICustomExport {
    protected static readonly String[] EMPTY = { };
    protected abstract String[] ControlFlowDefaults { get; }
    protected abstract String[] LinqDefaults { get; }
    protected abstract String[] VisibilityDefaults { get; }
    protected virtual bool ReduceOpacityForArgumentValidationDefaults => false;

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
    public bool ReduceOpacityForArgumentValidation {
      get { return this.Store.GetBoolean(KeyName + "_ReduceOpacityForArgumentValidation", ReduceOpacityForArgumentValidationDefaults); }
      set { this.Store.SetValue(KeyName + "_ReduceOpacityForArgumentValidation", value); }
    }


    public LanguageSettings(String key, ITypedSettingsStore store)
      : base(store) {
      this.KeyName = key;
    }

    public IDictionary<String, object> Export() {
      return new Dictionary<String, object> {
        { KeyName + "_ControlFlow", ControlFlow },
        { KeyName + "_Linq", Linq },
        { KeyName + "_Visibility", Visibility },
        { KeyName + "_Enabled", Enabled },
        { KeyName + "_ReduceOpacityForArgumentValidation", ReduceOpacityForArgumentValidation }
      };
    }
  }
}
