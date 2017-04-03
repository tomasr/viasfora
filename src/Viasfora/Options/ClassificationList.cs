using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Winterdom.Viasfora.Options {
  public class ClassificationList {
    private ColorStorage storage;
    public IDictionary<String, ClassificationColors> classifications;

    public ClassificationList(ColorStorage colorStorage) {
      storage = colorStorage;
      classifications = new Dictionary<String, ClassificationColors>();
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
  }
}
