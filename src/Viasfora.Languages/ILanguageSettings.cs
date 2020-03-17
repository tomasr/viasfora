using System;

namespace Winterdom.Viasfora.Languages {
  public interface ILanguageSettings {
    String KeyName { get; }
    String[] ControlFlow { get; set; }
    String[] Linq { get; set; }
    String[] Visibility { get; set; }
    bool Enabled { get; set; }
    bool ReduceOpacityForArgumentValidation { get; set; }
    bool ReduceOpacityForMethodOverloads { get; set; }
    void Load();
    void Save();
  }
}
