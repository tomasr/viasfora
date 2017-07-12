using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Winterdom.Viasfora.Settings;

namespace Winterdom.Viasfora.Options {
  public class ClassificationList : ICustomExport {
    private ColorStorage storage;
    private IDictionary<String, ClassificationColors> classifications;

    public ClassificationList(ColorStorage colorStorage) {
      storage = colorStorage;
      classifications = new Dictionary<String, ClassificationColors>();
    }

    public void Load(params Type[] classificationDefinitions) {
      var classificationNames = ExtractClassificationNames(classificationDefinitions);
      Load(classificationNames);
    }

    public void Load(params String[] classificationNames) {
      classifications.Clear();

      Guid category = new Guid(FontsAndColorsCategories.TextEditorCategory);
      uint flags = (uint)(__FCSTORAGEFLAGS.FCSF_LOADDEFAULTS
                        | __FCSTORAGEFLAGS.FCSF_READONLY);
      var hr = storage.Storage.OpenCategory(ref category, flags);
      ErrorHandler.ThrowOnFailure(hr);

      try {
        foreach ( var classification in classificationNames ) {
          var colors = new ClassificationColors(classification);
          colors.Load(storage);
          classifications.Add(classification, colors);
        }
      } finally {
        storage.Storage.CloseCategory();
      }
    }

    public void Save() {
      Guid category = new Guid(FontsAndColorsCategories.TextEditorCategory);
      uint flags = (uint)(__FCSTORAGEFLAGS.FCSF_LOADDEFAULTS
                        | __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES);
      var hr = storage.Storage.OpenCategory(ref category, flags);
      ErrorHandler.ThrowOnFailure(hr);

      try {
        foreach ( var colors in classifications.Values ) {
          colors.Save(storage);
        }
      } finally {
        storage.Storage.CloseCategory();
      }
    }

    public Color Get(String classificationName, bool foreground) {
      ClassificationColors entry;
      if ( classifications.TryGetValue(classificationName, out entry) ) {
        return entry.Get(foreground);
      }
      return default(Color);
    }

    public void Set(String classificationName, bool foreground, Color color) {
      var obj = classifications[classificationName];
      obj.Set(color, foreground);
    }

    public IDictionary<String, object> Export() {
      Dictionary<String, object> values = new Dictionary<String, object>();
      foreach ( var key in this.classifications.Keys ) {
        var entry = this.classifications[key];
        var fg = entry.Get(true);
        var bg = entry.Get(false);

        values[key.Replace(" ", "..")] = new String[] {
          ColorTranslator.ToHtml(fg),
          ColorTranslator.ToHtml(bg)
          };
      }
      return values;
    }

    public void ExportValues(ISettingsStore store) {
    }

    private String[] ExtractClassificationNames(Type[] classificationDefinitions) {
      List<String> names = new List<String>();
      foreach ( var def in classificationDefinitions ) {
        var fields = def.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach ( var field in fields ) {
          var name = GetDefinitionName(field);
          if ( !String.IsNullOrEmpty(name) ) {
            names.Add(name);
          }
        }
      }
      return names.ToArray();
    }

    private String GetDefinitionName(FieldInfo field) {
      var nameAttr = field.GetCustomAttribute<NameAttribute>();
      if ( nameAttr != null ) {
        return nameAttr.Name;
      }
      return null;
    }

  }
}
