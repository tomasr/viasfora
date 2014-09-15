using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Winterdom.Viasfora.Outlining {
  public class FeatureOutliningManager : BaseOutliningManager {
    protected FeatureOutliningManager(ITextBuffer buffer) : base(buffer) {
    }

    public static IUserOutlining Get(ITextBuffer buffer) {
      return buffer.Properties.GetOrCreateSingletonProperty(() => {
        return new FeatureOutliningManager(buffer);
      });
    }

    public static IOutliningManager GetManager(ITextBuffer buffer) {
      return Get(buffer) as IOutliningManager;
    }
  }
}
