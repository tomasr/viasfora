using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.Viasfora.Outlining {
  public class UserOutliningManager : BaseOutliningManager {

    protected UserOutliningManager(ITextBuffer buffer) : base(buffer){

    }
    public static IUserOutlining Get(ITextBuffer buffer) {
      return buffer.Properties.GetOrCreateSingletonProperty(() => {
        return new UserOutliningManager(buffer);
      });
    }

    public static IOutliningManager GetManager(ITextBuffer buffer) {
      return Get(buffer) as IOutliningManager;
    }
  }
}
